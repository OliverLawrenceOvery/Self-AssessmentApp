# Self-AssessmentApp
Self-Assessment Revision Aid using Entity Framework and WPF


## Project Goal
The goal of this project was to create a service that allows the user to customise their own revision. It will offer a means for storing, updating and deleting personalised revision notes and creating associated multiple choice questions for self-assessment purposes. This will all be tracked by a personal profile, with a means to graphically display test results.


## Project Definition of Done

- [ ] All user stories are marked as completed and tested.
- [ ] All unit tests pass.
- [ ] The end product is pushed to GitHub by 14:00 on 10/11/2020
- [ ] The documentation is fully realised, with a comprehensive README describing sprint progress and outcomes.
- [ ] The end product is reviewed and approved.

## Sprint 1

![Screenshot](/README_Images/sprint1start.png)

### Sprint Review

The first sprint focused on the definition and initialisation of the database, achieved using the code-first approach because my project was primarily domain-layer driven. Additionally, such an approach allows for the option of easily adjusting table properties and relationships via a simple code migration whenever required.

Creation of the database required careful consideration of the two primary functionalities offered by the service: the ability to store resources as notes concisely and in an easy-to-retrieve fashion, and the flexibility to create whatever tests and test questions the user desires. For the former, it was decided to structure resource notes into main topics, each of which contain many sub-topics. For the latter, the following hierarchal structure was decided upon: test series' which would each be comprised of many tests, which would each in turn be comprised of test questions. Such a segregation would give the user great flexibility in being able to customise ranges of tests to cover any conceivable topic. Finally, it is envisaged that Sprint 2 will add login/register functionality to the service, and as such it was decided to create a User class which would be associated with an Account, in order to track and personalise the user's interaction with the service. The finalised class diagram implemented is displayed below:

![Screenshot](/README_Images/classdiagram.png)

Once migrated to the database, it was necessary to develop CRUD services to facilitate table querying. This was achieved in a way that conforms to both the Open/Closed principle and the principle of Dependency Inversion. The foundation for CRUD functionality in this application is a data service interface that defines all of the basic CRUD operations, implemented by a generic data service class that takes a generic-type parameter - in other words, it can be used to query any table when needed. The main benefit of this is that the generic data service class is open for extension by any other class that requires a more specific CRUD query. It was additionally decided to make the CRUD methods asynchronous in order to optimise the GUI performance later on down the line. These CRUD methods where appropriately unit tested, with the passing confirming that they had been implemented as intended. The structure of my overall CRUD manager is illustrated below:

![Screenshot](/README_Images/CRUDdiagram.png)

Attention was now turned towards developing a basic WPF GUI layer, consisting of a navigation bar which would allow the user to switch views as desired. It was decided to follow the MVVM (model-view-view-model) design pattern when structuring the views that the application would support. Such an approach is beneficial because it ensures that the application architecture is loosely coupled, and as such modification of any one aspect out of the view, view model or model will not directly affect the other aspects. With this in mind, separate views and view models were created for each of the home, profile, resource and test views stipulated in the user stories, and were linked together in the app.xaml file. Before designing the navigation bar, I wanted a class to track and set the current state of the application - in particular, the current view model displayed. To this end, a navigator class was created to achieve this function. The navigation bar was next designed, with each view button on it binded to a command class which could update the current view model on the navigator when clicked on. Finally, as per the appropriate user story, the default view of the application was set to the home view.


### Sprint Retrospective
In summary, this sprint was highly successful: the database was conceived and created according to the specification detailed in the user stories, and an easily extendable, flexible CRUD service was created to manage interactions between visual studio and the database. Structuring the WPF layer to conform to the MVVM design pattern was additionally achieved without much difficulty, however the navigation bar required more careful consideration. For example, ensuring that the home button was highlighted upon application start-up required the creation of an IsChecked binding on each button to achieve this. All user stories were deemed to be satisfied, and preparations have been made for Sprint 2, such as the development of a framework for authentication unit testing.

![Screenshot](/README_Images/sprint1end.png)

## Sprint 2

![Screenshot](/README_Images/sprint2start.png)

### Sprint Review
Sprint 2 focused on the development of the authentication and registration services, before moving onto the GUI and ensuring that each view linked to the appropriate CRUD manager services. 

The development of the authentication service class involved building up the Login and Register methods. The Login method uses the account data service to first check the database to see if the account defined by the inputted username exists. Assuming this condition is satisfied, the Login method next unhashes the stored associated password and verifies it against the inputted password. This is the last stage of validation and returns a success flag which can be passed up to the GUI to login to the application. The Register method incorporates a similar number of validation checks: first checking that the password and confirm password fields match, before checking to see if an account with the inputted username already exists. Assuming these checks are all passed, the password is hashed and a new user and account are created in the database. It was decided to make these methods asynchronous, such that the GUI can update while methods such as account creation are taking place. The methods detailed here were all appropriately unit tested and after every method passed testing, attention could be turned towards developing the GUI layer. The authentication service functionality is illustrated below:

![Screenshot](/README_Images/authenticationdiagram.png)

The Login view required fields for the user to input a username and password with the option to register a new account. Upon logging in successfully, the user would be redirected to the home view, which displays the username at the top alongside a welcome message describing the functionality of the application. The profile view needed to first display the username and email associated with the currently logged in user, alongside a list of all personal test results underneath. These test results would be tied to a graph on the right, such that the graph would be populated by results relating to a specific test series as chosen by the user. This presented a few issues during development, most notably working out how to implement this graph functionality and ensuring that the graph showed in real time the test results relating to the test series chosen. This former issue was solved by simply installing the OxyPlot NuGet package which does a lot of the hard work for me – it simply needs a binding to a series of test points you wish to display, which was ideal given the time constraints of this sprint. The latter issue of displaying the correct test results for the test series chosen was rectified by using a binding on the points with a property changed trigger, such that when a test series was chosen the appropriate test results were fetched from the database and displayed on the graph in real time.

Regarding the resource view, it was required in the user stories that the user could select a particular main topic, and then the associated sub-topics would appear in the neighbouring list, and then selection of a sub-topic would populate the content fields. This functionality was achieved smoothly, and involved the use of a couple of list views to display the main and sub topics, alongside some text blocks to display the associated sub topic data. Following on from this, create, update and delete functionality were implemented for the sub-topics allowing for fully customisable sub-topics, although there was not enough time in this sprint to implement the same functionality for the main topics and will instead be pushed back to the final sprint.

Finally, the test view needed to be fully customisable with regards to the ability for the user to create test series’, tests associated with these series and questions associated with these tests. Next, the ability for the user to search through all the tests was added, with the additional feature that the tests were filtered in real-time according to the current input string. This was implemented using a simple method query which was executed every time the user inputted a new character to simply check for all tests that included the input string. Once a non-zero number of possible tests are shown to the user, they can be clicked on which populates a text box underneath and allows the user to start the selected test. I realised that I did not have enough time to implement a separate view for completing the test, and so I went around this by adjusting visibilities of elements – as soon as the user selected to start a test, all of the aforementioned views would collapse and disappear from view, and a view representing the current question would become visible. As soon as the user finished their test, the question view would collapse and the original test view would reappear again. Finally, a test result is created upon completing the test with the total test mark as calculated in the test view model, which can then be visualised on the graph in the profile view upon clicking the associated test series. The end result of some of these views is illustrated below:

![Screenshot](/README_Images/profileview.png)
![Screenshot](/README_Images/resourcesview.png)
![Screenshot](/README_Images/testsview.png)



### Sprint Retrospective
In summary, this sprint was noticably more intensive than the first, with the implementation of the required GUI functionality presenting numerous issues over the course of the development. It was perhaps naive to come into this sprint expecting it to go completely smoothly, and fortunately all the issues that presented themselves were fixed in a manner that did not overly convolute the code, with solutions that did not cause a cascade of subsequent issues/errors. SOLID principles were maintained throughout; for example Dependency Inversion was adhered to by a complete reliance on interfaces over classes - evidenced for example in the view model constructors, where interfaces for the authenticator and navigator were passed in as opposed to class instances. The Interface Segregation Principle was adhered through the addition of some new test services with custom CRUD methods, for example to retrieve all of the test results for a specific user and specific test series. The backbone of these CRUD services were interfaces for each set of customised CRUD methods, and as such multiple interfaces were used rather than a reliance on a single large interface. Going into sprint 3, error handling and field validation checks will be the primary focus to ensure that the user can only use these CRUD services when they input e.g. a valid test name, and additionally to prevent the user from making duplicate objects.


![Screenshot](/README_Images/sprint2end.png)

## Sprint 3

![Screenshot](/README_Images/sprint3start.png)

### Sprint Review
The focus for Sprint 3 was on optimising the robustness of the application, with regards to preventing the user from being able to crash the application and from creating resources, tests and questions with missing fields. Compared with the content described in Sprint 2, this functionality was notably simpler to implement and took the form of message boxes warning the user to populate all the relevant fields before being able to access any of the CRUD functionality within the application business layer. To counteract missing fields, my initial check only involved null checking all of the fields. A complication arose however in the scenario where a user enters something in a field, but then removes all of the text and attempts to press e.g. the create button. In this scenario, the field has an empty string value rather than a null value and this is something that subsequently needed to be included in the error checking for each field.

Additionally, as per the user stories a mechanism was implemented to prevent the user from creating duplicate main topics, test series' and tests which involved some simple custom READ operations to check if one of the aforementioned objects already existed in the database. These operations were subsequently unit tested and passed. Following on from this there was a focus on refactoring code, which took the form of reducing the repeatibility of code by creating functions that could be called by any part of the code.

### Sprint Retrospective
In summary, this sprint focused primarily on investigating the edge cases and error handling that the user could (accidentally or intentionally) encounter when using the application. The end of sprint 2 left me with a product with buttons that had direct access to CRUD methods upon being clicked, without any error checking and as such the application was very flimsy in that regard. Sprint 3 has rectified this by introducing null and empty string checking for the majority of fields, supplemented with informative message boxes displayed to the user. Certain text boxes such as the different content sub-sections have been allowed to be populated by just an empty string, to give the user the freedom to display and store as much information as they would like for each sub-topic.

![Screenshot](/README_Images/sprint3end.png)


## Project Retrospective

The undertaking and scope of this project was hugely rewarding for my understanding of object-oriented programming and project management. Not only did it enable me to apply my already existing knowledge to creating a resource that I may end up iterating upon and using in the future, but also gave me a valuable insight into the SCRUM process of project development and to this idea of letting the project board and testing drive a product's creation. 

### What have I learned?
Specific to C# functionality, this project has solidifed my understanding of the SOLID principles and how they should be adhered to in practice. For example, I observed the huge benefit in subscribing to the principle of Dependency Inversion by creating a dependency on a hierachal structure and interfaces rather than concrete class definitions, both in the form of greater code extendability and flexibility, and the benefit it had on unit testing my CRUD and authentication services. Specific to Unit Testing, I learnt about and implemented Moq testing which was used for my authentication service testing, because of the dependency of my IAuthenticationService interface on IPasswordHasher and IAccountService interfaces. This greatly improved the ease at which I was able to test this service, because Moq testing allowed me to create mock objects to represent these interfaces depended upon.

### What would I do differently moving forward?
Moving forward and for future projects, I need to place more of an emphasis on unit testing and test driven development early on in the project: much of my initial unit testing occured towards the end of Sprint 1 after much of the CRUD functionality was already implemented, whereas ideally it should be the other way around such that the code is more robust due to it being written in response to test cases (which are in direct response to the written user stories). Ultimately, the user stories are satisfied and user requirements are met when the appropriate testing is done and passed and so this dependency of the code on the tests rather than vice versa is something that I need to bare in mind moving forward. 

Additionally, I should gain more of an awareness of time constraints and not attempting to create too much functionality and too many tasks to achieve for a given sprint. For example, Sprint 2 was where the bulk of my GUI and CRUD service linking functionality was developed and there was very little spare time allocated for that sprint for the scenario where any serious bugs/errors were encountered. Fortunately, all of the bugs and errors that appeared during that sprint although potentially major in severity were relatively easy to replicate and fix. However, the risk was still there that a lot of the functionality that I wanted to implement in that sprint could have been delayed potentially until the last sprint due to one severe bug appearing. In hindsight, I should have divided the GUI work more effectively and equally between Sprint 2 and 3
and this something I will bare strongly in mind moving forward.
