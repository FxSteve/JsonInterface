# JsonInterface
C# strongly-typed interfaces for json objects

---

JsonInterface is a high-performance interface builder that enables interaction with a json object.  The intended goal is to allow interaction with a portion of a json object without knowledge or modification of the remainder of the object.

The heavy lifting is done by Newtonsoft's Json component and Castle Dynamic Proxy.

# Getting Started.

The json specification defines 6 types.  For the purposes of JsonInterface, these distill into 3 types.  
  1.  Primitive types
  2.  Objects
  3.  Arrays/Lists
  
## Primitive types

Primitive types encapulates the json types of string, number, boolean, and null as well as types such as dates or guids that serialize/deserialize to a single value.  JsonInterface requires all types to be nullable, so a type such as int must be specified as 
```
  int? MyValue { get; set; }
  Nullable<int> MyValue2 { get; set; }
```

## Objects (`IJsonObject`)

Objects in JsonInterface refers to an interface that inherits from IJsonObject.
```
public interface IMyJsonObject : IJsonObject 
{
  int? MyIntValue { get; set; }
  IMyOtherJsonObject OtherObject { get; set; }
  IJsonList<int?> MyListOfInts { get; set; }
  IJsonList<IMyListItem> MyListOfItems { get; set; }
  IJsonList<IJsonList<IMyChildListItem>> MyListOfList { get; set; }
}
```

## Arrays/Lists (`IJsonList<T>`)

Arrays are lists of a primitive type, objects of a type, or arrays of a type.  All items in a list must be the same type, or the related value might be null or throw an exception.

---

Once you create your compatible interface definition, create a new instance of the interface by
```
var myInterface = JsonInterfaceFactory.Create<IMyJsonObject>();
```

...

