void Number()
{
    int population = 67_000_000;
    long distance = 384_400_000L;
    short temperature = -40;
    byte red = 255;

    double pi = 3.141592653589793;
    float gravity = 9.81f;
    decimal price = 19.99m;
    Console.WriteLine(
        $"""
        population  {population}
        distance    {distance}
        temperature {temperature} 
        red         {red} 
        pi          {pi}
        gravity     {gravity}
        price       {price}
        """
    );
}

void String()
{
    char newline = '\n';
    char unicode = '\u0041'; // 'A'

    var greeting = "Hello, World!";
    var dec = 42;

    string message = $"Found {dec} items"; // interpolated string
    string path = @"C:\Users\docs\file.txt"; // verbatim string
    string json = """
        { "name": "Alice", "age": 30 }
        """; // raw string literal
    string raw = $"""
        Found {dec} items in "{greeting}"    
        """; // raw + interpolated

    Console.WriteLine(
        $"""
        newline  {newline}
        unicode  {unicode}
        greeting {greeting} 
        dec      {dec} 
        message  {message}
        path     {path}
        json     {json}
        raw      {raw}
        """
    );
}

void HelloWorld()
{
    Console.WriteLine($"The current time is {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    string name = "墨七";
    Console.WriteLine($"Hello,{name}!");
    var name2 = "墨七2";
    name2 = name2 + "3";
    Console.WriteLine("Hello," + name2 + "! ");

    String();
    Number();
}

HelloWorld();
