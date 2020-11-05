# Self-AssessmentApp
Self-Assessment Revision Aid using Entity Framework and WPF


## Project Goal
The goal of this project is to create a service that allows the user to customise their own revision. It will offer a means for storing personalised revision notes and creating associated multiple choice questions for self-assessment purposes.

## Sprint 1

### Sprint Review
The first sprint focused on the definition and initialisation of the database, achieved using the code-first approach because my project was primarily domain-layer driven. Additionally, such an approach allows for the option of easily adjusting table properties and relationships via a simple code migration whenever required.

Creation of the database required careful consideration of two primary functionalities offered by the service: the ability to store resources in the form of notes concisely and in an easy to retrieve fashion, and the flexibility and freedom to create whatever tests and test questions the user desired. For the former, it was decided to structure resource notes into main topics, each of which contain many sub-topics. For the latter, the following hierarchal structure was decided upon: test series' which would each be comprised of many tests, which would each in turn be comprised of test questions. Such a segregation would give the user great flexibility in being able to customise ranges of tests to cover any conceivable topic. Finally, it is envisaged that Sprint 2 will add login/register functionality to the service, and as such it was decided to create a User class which would be associated with an Account, in order to track and personalise the user's interaction with the user.

Once migrated to the database, it was necessary to develop CRUD services to facilitate table querying throughout the application. This was achieved in a way that conforms to both the Open/Closed principle and the principle of Dependency Inversion. The foundation for CRUD functionality in this application is a data service interface that defines all of the basic CRUD operations, implemented by a generic data service class that takes a generic-type parameter - in other words, it can be used to query any table when needed. The main benefit of this is that the generic data service class is open for extension by any other class that requires a more specific CRUD query. It was additionally decided to make the CRUD methods asynchronous in order to optimise the GUI performance later on down the line. These CRUD methods where appropriately unit tested, with the passing confirming that they had been implemented as intended.

Attention was now turned towards developing a basic WPF GUI layer, consisting of a navigation bar which would allow the user to switch views as desired. It was decided to follow the MVVM (model-view-view-model) design pattern when structuring the views that the application would support. Such an approach is beneficial because it ensures that the application architecture is loosely coupled, and as such modification of any one aspect out of the view, view model or model will not directly affect the other aspects. Additionally, it ensures that the code is more testable because test cases can be written that do not need to reference the view layer. With this in mind, separate views and view models were created for each of the home, profile, resource and test reviews stipulated in the user stories, and were linked together in the app.xaml file. Before designing the navigation bar, I wanted a class to track and set the current state of the application - in particular, the current view model displayed. To this end, a navigator class was created to achieve this function. The navigation bar was next designed, with each view button on it binded to a command class which could update the current view model on the navigator when clicked on. Finally, as per the appropriate user story, the default view of the application was set to the home view.


### Sprint Retrospective
In summary, this sprint was highly successful: the database was conceived and created according to the specification detailed in the user stories, and an easily extendable, flexible CRUD service was created to manage interactions between visual studio and the database. Structuring the WPF layer to conform to the MVVM design pattern was additionally achieved without any difficulty, however the navigation bar required more careful consideration. For example, ensuring that the home button was highlighted upon application start-up required the creation of an IsChecked binding on each button to achieve this. All user stories were deemed to be satisfied, and preparations have been for made for Sprint 2, such as the development of a framework for authentication unit testing.



## Sprint 2

### Sprint Review

### Sprint Retrospective

## Sprint 3

### Sprint Review

### Sprint Retrospective
