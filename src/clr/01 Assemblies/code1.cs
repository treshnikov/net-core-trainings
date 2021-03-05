using System;
using MyTypes;

public class Program
{
    public static void Main()
    {
        var t = new MyTypes.MyRefType();
        var text = t.DoWork(); 
        Console.WriteLine(text);       
        Console.WriteLine("123");       
    }
}