Feature: Sample feature to show binding a table to an object graph

Scenario: Sample to show binding a table to a complex type
	Given I have the following customers
	| FirstName | LastName | Address.Address1 | Address.Address2 | Address.Town | Address.PostCode |
	| Kevin     | Kuszyk   | Address 1        | Address 2        | Town         | LS1 1AB          |
	| John      | Smith    | A house          | Somewhere        | Else         | EL1 1SE          |

	And I do something

	Then I should be able to retrieve the saved customers
