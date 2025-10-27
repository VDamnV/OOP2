using System;

namespace lab_4;

public delegate int FindCharIndex(string str, char c);

static class Program
{
    static void Main()
    {
        Console.WriteLine("==============\nЧастина 1\n==============");

        FindCharIndex anonymous = delegate (string str, char c)
        {
            return str.IndexOf(c);
        };
        int anonResult = anonymous.Invoke("Hello world", 'o');
        Console.WriteLine($"Результат анонімного методу (пошук 'o'): {anonResult}");

        FindCharIndex lambda = (str, c) => str.IndexOf(c);
        
        int lambdaResult = lambda.Invoke("C# programming", 'z');
        Console.WriteLine($"Результат лямбда-виразу (пошук 'z'): {lambdaResult}");

        Console.WriteLine("\n==============\nЧастина 2 і 3\n==============");

        var myStack = new MyStack<int>(3); // Створюємо стек з обмеженою ємністю 3

        myStack.StackOverflow += OnStackOverflowHandler;
        
        myStack.ElementPushed += OnElementPushed;
        myStack.ElementPopped += OnElementPopped;


        myStack.Push(10);
        myStack.Push(20);
        myStack.Push(30);

        Console.WriteLine($"\nСтек заповнено. Кількість елементів: {myStack.Count}");
        Console.WriteLine($"Верхній елемент: {myStack.Peek()}");

        Console.WriteLine("\nСпроба додати елемент '40' у повний стек:");
        myStack.Push(40); 

        Console.WriteLine($"\nКількість елементів після спроби: {myStack.Count}");

        Console.WriteLine($"\nВидалено: {myStack.Pop()}");
        Console.WriteLine($"Видалено: {myStack.Pop()}");
        Console.WriteLine($"Залишився верхній елемент: {myStack.Peek()}");
        Console.WriteLine($"Кількість елементів: {myStack.Count}");
    }

    static void OnStackOverflowHandler(object? sender, StackOverflowEventArgs e)
    {
        Console.WriteLine($"*** ПОДІЯ: ПЕРЕПОВНЕННЯ СТЕКУ! ***");
        Console.WriteLine($"   Не вдалося додати елемент: {e.Element}");
        Console.WriteLine($"   Максимальна ємність стеку: {e.Capacity}");
        if (sender is MyStack<int> stack)
        {
            Console.WriteLine($"   Об'єкт-ініціатор: {stack.GetType().Name}, поточний розмір: {stack.Count}");
        }
    }
    
    static void OnElementPushed(object? sender, StackChangedEventArgs<int> e)
    {
        Console.WriteLine($"(Подія: Елемент '{e.Element}' додано до стеку)");
    }
    
    static void OnElementPopped(object? sender, StackChangedEventArgs<int> e)
    {
        Console.WriteLine($"(Подія: Елемент '{e.Element}' видалено зі стеку)");
    }
}