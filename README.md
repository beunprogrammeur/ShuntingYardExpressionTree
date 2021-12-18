# ShuntingYardExpressionTree
## A C# Shunting yard implementation that allows for re-evaluation of the input result.

### Supported operators in examples:
```
1 + 2
1 - 2
-1 + 2
-1 + -2
3 * 2 
3 / 2
3 % 2
3 ^ 2
pi / 180 // assuming you register the pi variable
```

### Example 1:
```
// defaults to invariant culture if none is given
var parser = new Parser();

var operand = parser.ParseFormula("1 + 2 * 5");

// prints 11
Console.WriteLine(operand.Value);
```
### Example 2:
```
var parser = new Parser();
parser.RegisterVariable(new RelayProperty("unix", () => DateTimeOffset.Now.ToUnixTimeSeconds()));

var operand = parser.ParseFormula("unix % 3");

while(true)
{
    Console.WriteLine(operand.Value);
    Thread.Sleep(1000);
}

// prints:
// 0
// 1
// 2
// 0
// 1
// 2
// 0
// etc.
```

The point of having this reevaluation is that parsing the expression is more resource intensive than recalculating the value of the expression.
Another reason is that as seen in ```example 2```, the variable ```unix``` changes over time. This means that the Value of the expression also changes over time.

An example of using this:
Assume you have a graphical application. You draw a recangle on an arbitray location on this screen. You also want to draw a circle, but that circle has to be at the center of this rectangle. You can move this rectangle's position somehow (arrow keys, mouse, you name it).

Assume you register the following variables with the parser:
```
parser.RegisterVariable(new RelayProperty("x", () => rectangle.Position.X));
parser.RegisterVariable(new RelayProperty("y", () => rectangle.Position.Y));
parser.RegisterVariable(new RelayProperty("width", () => rectangle.Width));
parser.Registervariable(new RelayProperty("height", () => rectangle.Height));
```

Also assume the following code:
```
var xPosition = parser.ParseFormula("x + width / 2");
var yPosition = parser.ParseFormula("y + height / 2");

while(true)
{
   circle.Position.X = xPosition.Value;
   circle.Position.Y = yPosition.Value;
   Thread.Sleep(16);
}
```

This would move the circle to the center position of the rectangle and update that position every 16 ms.

I made this mainly for having scripting freedom, where the user can make mathmatical changes to a program that implements this code, without changing the code base. (changes that can be evaluated often and fast with a relatively low system impact).

#### Simple benchmark results: (i5 4670k, debug mode)
parsing "1+2+3+4+5+6+7+8+9+0"
100.000 times in 1076ms
Requesting the value from the expression:
100.000 times in 62,3ms

# Added feature: expressions
I've added the possibility to assign a variable within the expression.
this means that the following will evaluate to an executable expression that does not return a result:

```
double myNumber;

var parser = new Parser();
parser.RegisterVariable(new ValueProperty("number", 0));
parser.RegisterVariable(new RelayProperty("myNumber", () => myNumber, value => myNumber = value));

var expression = parser.ParseExpression("number = number + 1; myNumber = number");

while(true)
{
    expression.Evaluate();
    // result:
    // myNumber: 1
    // myNumber: 2
    // myNumber: 3
    // myNumber: 4 etc.
}
```
## A more useful example of expressions
```
var rect = new Rectangle();
var circle = new Circle();
parser.RegisterVariable(new RelayProperty("rectangle.x", () => rect.X, value => rect.X = value));
parser.RegisterVariable(new RelayProperty("rectangle.y", () => rect.Y, value => rect.Y = value));
parser.RegisterVariable(new RelayProperty("rectangle.width", () => rect.Width, value => rect.Width = value));
parser.RegisterVariable(new RelayProperty("rectangle.height", () => rect.Height, value => rect.Height = value));
parser.RegisterVariable(new RelayProperty("circle.x", () => circle.X, value => circle.X = value));
parser.RegisterVariable(new RelayProperty("circle.y", () => circle.Y, value => circle.Y = value));

var expression = parser.ParseExpression("
    circle.x = rectangle.x + rectangle.width / 2
    circle.y = rectangle.y + rectangle.height / 2
");

while(true) {
    expression.Evaluate();
}

// when the rectangle would change its position, the circle would "stick" to its center point.
```