# CLU Pizza Samples
The following JSON files demonstrate a way to represent a typical LUIS pizza application with structured entities as different projects to be used together.

* **MicrosoftPizza - LUIS**: This is a LUIS application that includes a detailed structured entity that parses an order for individual pizza orders and each pizza order's individual characteristics and toppings.
* **MicrosoftPizzaLarge - CLU**: This is a CLU project that is only attempting to extract the spans for the top layer of pizza orders. 
* **MicrosoftPizzaSmall - CLU**: This is a CLU project that is only attempting to extract all the characteristics and toppings.

You can replicate the LUIS project by combining the responses from both the Large and Small CLU variations. 

For example, the Large CLU project can extract the pizza orders from the example "hey i want to order **a large supreme pizza add cheese no onions** plus **a large pepperoni pizza with sausage**".
The Small CLU project will then extract the sizes, types, toppings and modifiers. 

As a post-processing step you can associate which sizes, types, toppings, and modifiers belong to which pizza order based on which spans they fall into, predicted by the Large project.

You could alternatively extract this in sequence. First, extract the individual pizza order spans with the Large CLU project. Then, each span can be sent separately to the Small project to find out its characteristics.



