# Homeric
Mathematical expression compiler for .NET

### What is it?
This is a simple open-source library that helps to parse and calculate mathematical expressions. For example:

`"2 + 10 - 5 * 2" returns 2`, `"2 * x" returns 4 if x = 2` etc.

### Main features
* Homeric is lightweight. Everything you need to use it - just to reference "HomericLibrary.dll" file in your project.
* Homeric is very simple in its structure and exploitation.
* Homeric is quick if you need to calculate the same equation lots of times.
* Homeric has built-in almost all main mathematical functions.
* It's free:)
* ...

### How to use it

#### Get library
To start using HomericLibrary simple download dll file from [here] (homeric/HomericLibrary/Download/HomericLibrary.dll).
Although I try to watch this file to be up-to-date regularly, it might not be updated. If you see that its version is a bit old, you can create it by yourself. In this case you need
to clone this repository, export this project to Visual Studio and build it.

#### Add to project
To use Homeric in your project, simply add reference to it. Then you should add 
```c#
using HomericLibrary;
```
in the file where you plan to use Homeric.

#### Create expression

> Let we have expression 1: `"2 * Sin(3 * PI / 2) - 5 * Ln(E ^ 2)"` 
>
> and expression 2: `"4 * Cos(x) + 5 * Sqrt(y^2)", "x", "y"`

`HomericExpression` has two constructors:

> Methods signatures:
> ```c#
public HomericExpression(string expression)
public HomericExpression(string expression, params string[] vars)
>```

To create new expression instance, simply call one of them:
```c#
HomericExpression expr1 = new HomericExpression("2 * Sin(3 * PI / 2) - 5 * Ln(E ^ 2)");
```
-- this one will create new expression without variables.
```c#
HomericExpression expr2 = new HomericExpression("4 * Cos(x) + 5 * Sqrt(y^2)", "x", "y");
```
-- this one will create new expression with variables `x` and `y`. 

> Note that you must set variable names explicitly. We plan to make homeric detect variables automatically, 
but this feature will be added in future releases.

> Also, if the expression contains some errors (brackets placed incorrectly, expression contains symbols that are not allowed,
you try to add the same variable multiple times etc.), Homeric will throw `ArgumentException`.

#### How to calculate expressions and get results

> Methods signatures:
> ```c#
double Calculate()
double Calculate(params double[] values)
double Calculate(Dictionary<string, double> values)
```

To do this, simply call `Calculate` method. For example:
```c#
Console.WriteLine(expr1.Calculate());
```
-- after running this piece of code, we will have `-12` written in console.

```c#
Console.WriteLine(expr2.Calculate(Math.PI, 8));
```
-- after running this piece of code, the output will be `36`. This means that we want to get the value of expression 2 
when `x = PI = 3.14159...` and `y = 8`.

> Note that the order of variables' values in Calculate method should be the same as it was while creating the expression. If you want
to set variables' values in another order, you can use another variant of this method that has `Dictionary` as input argument:

```c#
var values = new Dictionary<string, double>();
values.Add("y", 8);
values.Add("x", Math.PI);
Console.WriteLine(expr2.Calculate(values));
```
-- this notation will give the same results as in the previous example.
