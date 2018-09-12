# specflow-table-to-complex-type-sample

Sample code for techtalk/SpecFlow#1217.

The `VerifyPropertiesAndCreateSet()` extension method allows a step like this:

```
Given I have the following customers
| FirstName | LastName | Address.Address1 | Address.Address2 | Address.Town | Address.PostCode |
| Kevin     | Kuszyk   | Address 1        | Address 2        | Town         | LS1 1AB          |
| John      | Smith    | A house          | Somewhere        | Else         | EL1 1SE          |
```

To bind a table to a complex type / object graph like this:

```
public class Customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Town { get; set; }
    public string PostCode { get; set; }
}
```

The code in the `TableExtensions` class has been copied over as is from one of our interal repros.  It needs some clean up before it's ready to be contributed back to the main SpecFlow project.
