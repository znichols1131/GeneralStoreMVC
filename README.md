# General Store MVC

Author: Zach Nichols

Due Date: January 20, 2022

Assignment: The **Eleven Fifty Academy General Store MVC** app requires students to revisit the General Store API project and reconstruct it as an MVC app. The following classes were required to have data models, controllers, and views for creation, viewing details, updating, and deletion:

- `Product` contains a unique ID, Name, Price, Number in Stock, and boolean indicating whether it is a food item. The Product list sorts by Name.

- `Customer` contains a unique ID, First Name, and Last Name. The Customer list sorts by First Name.

- `Transaction` contains a unique ID, Product reference via the ProductID foreign key, Customer reference via the CustomerID foreign key, and Product Count to track the quantity purchased. The Date Created is stored when the Transaction is initially created, while the Date Updated stores the date of the latest update. The Transaction list sorts by the most recent activity (Date Created or Date Updated).

In an effort to push the boundaries of my learning, I added the following features:

- `Transaction validation` methods ensure that Transactions can only be created/updated if the associated Product has enough inventory. Likewise, deleted Transactions return their inventory to the associated Product.

- `CartItem` class, controllers, and views allow users to add Products to a shopping cart (Cart Item Index page). This is accomplished by calling the Cart Item Create page from the Product Index page. While in the cart, Cart Items are considered unfinished Transactions. When the user clicks the button to complete purchases, the Cart Item controller will complete all valid Transactions and output a message explaining the remaining Cart Items. If the cart is empty, the button disappears.

## Resources

- [GitHub Repository](https://github.com/znichols1131/GeneralStoreMVC)
- [Assignment Requirements and Rubric](https://elevenfifty.instructure.com/courses/852/assignments/19086?return_to=https%3A%2F%2Felevenfifty.instructure.com%2Fcalendar%23view_name%3Dmonth%26view_start%3D2022-01-07)




