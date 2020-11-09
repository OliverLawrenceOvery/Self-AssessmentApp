# Self-AssessmentApp
Self-Assessment Revision Aid using Entity Framework and WPF


## Project Goal
The goal of this project is to create a service that allows the user to customise their own revision. It will offer a means for storing personalised revision notes and creating associated multiple choice questions for self-assessment purposes.

## Sprint 1

### Sprint Review
The first sprint focused on the definition and initialisation of the database, achieved using the code-first approach because my project was primarily domain-layer driven. Additionally, such an approach allows for the option of easily adjusting table properties and relationships via a simple code migration whenever required.

Creation of the database required careful consideration of two primary functionalities offered by the service: the ability to store resources in the form of notes concisely and in an easy to retrieve fashion, and the flexibility and freedom to create whatever tests and test questions the user desired. For the former, it was decided to structure resource notes into main topics, each of which contain many sub-topics. For the latter, the following hierarchal structure was decided upon: test series' which would each be comprised of many tests, which would each in turn be comprised of test questions. Such a segregation would give the user great flexibility in being able to customise ranges of tests to cover any conceivable topic. Finally, it is envisaged that Sprint 2 will add login/register functionality to the service, and as such it was decided to create a User class which would be associated with an Account, in order to track and personalise the user's interaction with the user.

![alt text](https://github.com/OliverLawrenceOvery/Self-AssessmentApp/tree/master/README_Images/blob/master/image.jpg?raw=true)

Once migrated to the database, it was necessary to develop CRUD services to facilitate table querying throughout the application. This was achieved in a way that conforms to both the Open/Closed principle and the principle of Dependency Inversion. The foundation for CRUD functionality in this application is a data service interface that defines all of the basic CRUD operations, implemented by a generic data service class that takes a generic-type parameter - in other words, it can be used to query any table when needed. The main benefit of this is that the generic data service class is open for extension by any other class that requires a more specific CRUD query. It was additionally decided to make the CRUD methods asynchronous in order to optimise the GUI performance later on down the line. These CRUD methods where appropriately unit tested, with the passing confirming that they had been implemented as intended.

Attention was now turned towards developing a basic WPF GUI layer, consisting of a navigation bar which would allow the user to switch views as desired. It was decided to follow the MVVM (model-view-view-model) design pattern when structuring the views that the application would support. Such an approach is beneficial because it ensures that the application architecture is loosely coupled, and as such modification of any one aspect out of the view, view model or model will not directly affect the other aspects. Additionally, it ensures that the code is more testable because test cases can be written that do not need to reference the view layer. With this in mind, separate views and view models were created for each of the home, profile, resource and test views stipulated in the user stories, and were linked together in the app.xaml file. Before designing the navigation bar, I wanted a class to track and set the current state of the application - in particular, the current view model displayed. To this end, a navigator class was created to achieve this function. The navigation bar was next designed, with each view button on it binded to a command class which could update the current view model on the navigator when clicked on. Finally, as per the appropriate user story, the default view of the application was set to the home view.


### Sprint Retrospective
In summary, this sprint was highly successful: the database was conceived and created according to the specification detailed in the user stories, and an easily extendable, flexible CRUD service was created to manage interactions between visual studio and the database. Structuring the WPF layer to conform to the MVVM design pattern was additionally achieved without any difficulty, however the navigation bar required more careful consideration. For example, ensuring that the home button was highlighted upon application start-up required the creation of an IsChecked binding on each button to achieve this. All user stories were deemed to be satisfied, and preparations have been made for Sprint 2, such as the development of a framework for authentication unit testing.



## Sprint 2

### Sprint Review
Sprint 2 focused on the development of the authentication and registration services, before moving onto the GUI and ensuring that each view linked to the appropriate CRUD manager services. 

The development of the authentication service class involved building up the pre-defined Login and Register methods. The Login method uses the account data service to first check the database to see if the account defined by the inputted username exists. Assuming this condition is satisfied, the Login method next unhashes the stored associated password and verifies it against the inputted password. This is the last stage of validation and returns a success flag which can be passed up to the GUI to login to the application. The Register method incorporates a similar number of validation checks: first checking that the password and confirm password fields match, before checking to see if an account with the inputted username already exists. Assuming these checks are all passed, the password is hashed and a new user and account are created in the database. It was decided to make these methods async, such that the GUI can update while methods such as account creation are taking place. The methods detailed here were all appropriately unit tested and after every method passed testing, attention could be turned towards developing the GUI layer.

The development of the GUI this centred going through each view and implementing functionality described by the associated user stories. For example, the Login view required fields for the user to input a username and password with the option to register a new account. This register view required fields for a username, password, password confirmation and an email, and the user could navigate to and from the login and register views simply by pressing the appropriate button. Upon logging in successfully, the user would be redirected to the home view, which displays the username at the top alongside a welcome message describing the functionality of the application. The profile view needed to first display the username and email associated with the currently logged in user, alongside a list of all personal test results underneath. These test results would be tied to a graph on the right, such that the graph would be populated by results relating to a specific test series as chosen by the user. This presented a few issues during development, most notably working out how to implement this graph functionality and ensuring that the graph showed in real time the test results relating to the test series chosen. This former issue was solved by simply installing the OxyPlot NuGet package which does a lot of the hard work for me – it simply needs a binding to a series of test points you wish to display, which was ideal given the time constraints of the this sprint. The latter issue of displaying the correct test results for the test series chosen was rectified by using a binding on the points with a property changed trigger, such that when a test series was chosen the appropriate test results were fetched from the database and displayed on the graph in real time.

Regarding the resource view, it was required in the user stories that the user could select a particular main topic, and then the associated sub-topics would appear in the neighbouring list, and then selection of a sub-topic would populate the appropriate content fields. This functionality was achieved smoothly, and involved the use of a couple of list views to display the main and sub topics, alongside some text blocks to display the associated sub topic data. Following on from this, create, update and delete functionality were implemented for the sub-topics allowing for fully customisable sub-topics, although there was not enough time in this sprint to implement the same functionality for the main topics.

Finally, the test view needed to be fully customisable with regards to the ability for the user to create test series’, tests associated with these series and questions associated with these tests. As such, create functionality was needed for all three of these, encapsulated within clearly labelled, bordered grids on the GUI. The linking of all these grids to the appropriate services was time-consuming but ultimately successful. Next, the ability for the user to search through all the tests was added, with the additional feature that the tests were filtered in real-time according to the current input string. This was implemented using a simple method query which was executed every time the user inputted a new character to simply check for all tests that included the input string. Once a non-zero number of possible tests are shown to the user, they can be clicked on which populates a text box underneath and allows the user to start the selected test. I realised that I did not have enough time to implement a separate view for completing the test, and so I went around this by adjusting visibilities of elements – as soon as the user selected to start a test, all of the aforementioned views would collapse and disappear from view, and a view representing the current question would become visible. This method of implementation was beneficial because it was easy to bind grid visibilities to the button that starts the test. The mentioned view for each question simply consists of text fields that are populated by the question text and question options, and the user can only progress onto the next question once an option is selected. Behind the scenes in the view model code, a check is made between the option selected and the correct answer for that question and the total test mark is updated if the user selects the correct answer.



### Sprint Retrospective
In summary, this sprint was noticably more intensive than the first, with the implementation of the required GUI functionality presenting numerous issues over the course of the development. It was perhaps naive to come into this sprint expecting it to go completely smoothly, and fortunately all the issues that presented themselves were fixed in a manner that did not overly convolute the code, with solutions that did not cause a cascade of subsequent issues/errors. SOLID principles were maintained throughou; for example Dependency Inversion was adhered to by a complete reliance on interfaces over classes - evidenced for example in the view model constructors, where interfaces for the authenticator and navigator were passed in as opposed to class instances. The Interface Segregation Principle was adhered through the addition of some new test services with custom CRUD methods, for example to create a new test result for a specific test and user, and to retrieve all of the test results for a specific user and specific test series. The backbone of these CRUD services were interfaces for each set of customised CRUD methods, and as such multiple interfaces were used rather than a reliance on a single large interface.

## Sprint 3

### Sprint Review

### Sprint Retrospective
