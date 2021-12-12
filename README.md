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

var operand = parser.Parse("1 + 2 * 5");

// prints 11
Console.WriteLine(operand.Value);
```
### Example 2:
```
var parser = new Parser();
parser.RegisterVariable("unix", () => DateTimeOffset.Now.ToUnixTimeSeconds());

var operand = parser.Parse("unix % 3");

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
parser.RegisterVariable("x", () => rectangle.Position.X);
parser.RegisterVariable("y", () => rectangle.Position.Y);
parser.RegisterVariable("width", () => rectangle.Width);
parser.Registervariable("height", () => rectangle.Height);
```

Also assume the following code:
```
var xPosition = parser.Parse("x + width / 2");
var yPosition = parser.Parse("y + height / 2");

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
