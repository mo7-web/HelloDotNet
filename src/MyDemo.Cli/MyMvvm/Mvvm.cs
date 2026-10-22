namespace MyDemo.Cli.MyMvvm;

using System;

public class Demo
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Demo(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Age: {Age}");
    }
}
