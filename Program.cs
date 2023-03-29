using System;
using System.Linq;
using System.IO;

namespace FiveLetters;

public static class Program
{
    const string helpText = "\thelp (пом) - вывести спискок комманд\n" +
                            "\treset (сброс) - сбросить все фильтры\n" +
                            "\texcl (искл) [буквы] - исключить слова с указанными буквами\n" +
                            "\treq (надо) [буквы] - оставить только слова с указанными буквами\n" +
                            "\tshow (показ)- показать выбранные слова\n" +
                            "\trand (случ)- показать случайное слово\n" +
                            "\tqu (вых) - выход";
    static string[] allWords  = new string[0];  // = ReadAllWords(allWordsFileName);
    //static string[] filtredWords = new string[allWords.Length];
    //static char[] excluded = new char[0];
    //static char[] required = new char[0];
    //static 
    enum command
    {
        help,
        reset,
        exclude,
        require,
        show,
        random,
        quit,
        uncknown
    }

    public static void Main()
    {
        //SelectFivelettersWords("букваС1.txt","5с1.txt");
        //SelectFivelettersWords("букваС2.txt","5с2.txt");
        //AllToOneFile();


        //string allWordsFileName = Directory.GetCurrentDirectory()+"/5letters.txt";
        //string allWordsFileName = Directory.GetCurrentDirectory() + "\\Letters\\5lettersshort.txt";
        string allWordsFileName = Directory.GetCurrentDirectory() + "\\Letters\\5letters.txt";
        allWords = ReadAllWords(allWordsFileName);

        Console.Clear();
        Console.WriteLine("Загружено {0} слов ", allWords.Length);
        Console.WriteLine("Случайное слово: {0}", GetRandonWord(allWords));

        PrintHelp();
        UserCommandLoop();


    }

    // запускаем цикл обработки пользовательских комманд
    private static void UserCommandLoop()
    {
        string[] filtredWords = new string[allWords.Length];
        allWords.CopyTo(filtredWords, 0);
        char[] excluded = new char[0];
        char[] required = new char[0];

        string userText = string.Empty;
        bool exit = false;
        do
        {
            Console.Write("Введите комманду: ");
            userText = Console.ReadLine()!;
            command com = GetCommand(userText);
            //Console.WriteLine(com.ToString());
            switch (com)
            {
                case command.help:          // +++
                    // пишем хелп
                    PrintHelp();
                    break;
                case command.show:          // +++
                    // показываем выборку
                    if (filtredWords.Length > 100)   // если слов много надо уточнить
                    {
                        Console.Write("В выборке больше ста слов. Показать? ( Y | N ): ");
                        string ansver = Console.ReadLine()!;
                        ansver = ansver.Trim().ToUpper();
                        if (ansver == "Y") PrintWords(filtredWords);
                    }
                    else PrintWords(filtredWords);
                    break;
                case command.random:        // +++
                    // выдаём случайное слово
                    Console.WriteLine(GetRandonWord(filtredWords));
                    break;
                case command.reset:         // +++
                    // сбрасываем сортировки
                    excluded = new char[0];
                    required = new char[0];
                    filtredWords = new string[allWords.Length];
                    allWords.CopyTo(filtredWords, 0);
                    PrintStatus(filtredWords, excluded, required);
                    break;
                case command.quit:          // +++
                    // выходим
                    exit = true;
                    break;
                case command.require:
                    char[] reqChars = GetParametres(userText);
                    // фильтруем по необходимым буквам
                    RequiredLetters(ref filtredWords, reqChars);
                    // добавляем отфильтрованные буквы в общий список
                    AddAndSortLetters(ref required, reqChars);
                    // Выводим собщение
                    PrintStatus(filtredWords, excluded, required);   
                    break;
                case command.exclude:
                    char[] exclChars = GetParametres(userText);
                    // фильтруем по лишним буквам
                    ExcludeLetters( ref filtredWords, exclChars);
                    // добавляем отфильтрованные буквы в общий список
                    AddAndSortLetters(ref excluded, exclChars);
                    // Выводим собщение
                    PrintStatus(filtredWords, excluded, required);
                    break;
                case command.uncknown:
                    // комманда не распознана
                    Console.WriteLine("Комманда не распознана. Повторите.");
                    break;
            }
        } while (!exit);
    }


    // Печать статуса (всего слов, исключенные и необходимые символы)
    private static void PrintStatus(string[] words, char[] excluded, char[] reqiured)
    {
        string exclMsg = string.Empty;
        if (excluded.Length == 0) exclMsg = "Исключенных символов нет";
        else exclMsg = "Исключенные символы: " + CharListToString(excluded);

        string reqiurMsg = string.Empty;
        if (reqiured.Length == 0) reqiurMsg = "Обязательных символов нет";
        else reqiurMsg = "Обязательные символы: " + CharListToString(reqiured);

        Console.WriteLine("Выбрано {0} слов. {1}. {2}.", words.Length, exclMsg, reqiurMsg);
    }


    // получаем тип комманды из строки веденной пользователем
    private static command GetCommand(string input)
    {
        input = input.Trim().ToLower();

        if (input == "help" || input == "пом") return command.help;
        else if (input == "reset" || input == "сброс") return command.reset;
        else if (input.IndexOf("excl") == 0 || input.IndexOf("искл") == 0) return command.exclude;
        else if (input.IndexOf("req") == 0 || input.IndexOf("надо") == 0) return command.require;
        else if (input == "show" || input == "показ") return command.show;
        else if (input == "rand" || input == "случ") return command.random;
        else if (input == "qu" || input == "вых") return command.quit;
        //Console.WriteLine(input);
        return command.uncknown;
     }

    // Извлекаем символы из комманды поользователя
    private static char[] GetParametres(string input)
    {
        // 1. обрезаем комманду, оставляем только символы
        int del = 4;
        if (input.IndexOf("req") == 0) del = 3;
        //else if (input.IndexOf("req") == 0)    del=3;
        input = (input.Remove(0, del)).Trim();

        // 2. создаем из остатка массив символов
        char[] chars = input.ToCharArray();

        // 3. из этого массива оставляем только "наши" буквы
        char[] result = new char[0];
        foreach (char ch in chars)
        {
            if (ch >= 'а' && ch <= 'я')
            {
                Array.Resize(ref result, result.Length + 1);
                result[result.Length - 1] = ch;
            }
        }

        return result;
    }

    // Печатаем хелп
    private static void PrintHelp()
    {
        Console.WriteLine(helpText);
    }

    // выбираем только слова содержащие указанные буквы
    private static void RequiredLetters(ref string[] words, char[] required)
    {
        string[] tempWords = new string[words.Length];
        int len = 0;
        foreach (string word in words)
        {

            if (AllCharsExist(word, required))
            {
                tempWords[len] = word;
                len++;
            }
        }

        string[] filtred = new string[len];
        for (int i = 0; i < len; i++)
        {
            filtred[i] = tempWords[i];
        }
        words = filtred;
    }

    // Проверяем на наличие всех букв
    private static bool AllCharsExist(string word, char[] letters)
    {
        foreach (char ch in letters)
        {
            if (word.IndexOf(ch) < 0)
                return false;
        }
        return true;
    }

    // Нормальный список из букв
    private static string CharListToString(char[] symbols)
    {
        string res = symbols[0].ToString();
        for (int i = 1; i < symbols.Length; i++)
            res += ", " + symbols[i];
        return res;
    }


    // добавляем буквы к списку
    private static void AddAndSortLetters(ref char[] exist, char[] added)
    {
        char[] newList = new char[exist.Length + added.Length];
        exist.CopyTo(newList, 0);
        added.CopyTo(newList, exist.Length);

        Array.Sort(newList);
        exist = newList;
    }

    // печатаем список слов
    private static void PrintWords(string[] words)
    {
        int n = 0;
        foreach (string w in words)
        {
            if (n % 8 == 0) Console.WriteLine();
            Console.Write(w + " \t ");
            n++;
        }
        Console.WriteLine();
    }

    // выбираем слова без указанных букв
    private static void ExcludeLetters(ref string[] words, char[] excluding)
    {
        string[] tempWords = new string[words.Length];
        int len = 0;
        foreach (string word in words)
        {
            if (!CharsExist(word, excluding))
            {
                tempWords[len] = word;
                len++;
            }
        }

        string[] filtred = new string[len];
        for (int i = 0; i < len; i++)
        {
            filtred[i] = tempWords[i];
        }
        words = filtred;

        // !!! вернуть список исключенных букв
        //return new char[0];
    }

    // есть ли буквы в слове
    private static bool CharsExist(string word, char[] letters)
    {

        foreach (char ch in letters)
        {
            if (word.IndexOf(ch) >= 0) return true;
        }

        return false;
    }



    // выбираем случайное слово из массива
    private static string GetRandonWord(string[] words)
    {
        return words[new Random().Next(words.Length)];
    }

    // делаем строковый массив из слов(строк) в файле
    private static string[] ReadAllWords(string fileName)
    {
        int len = CountLines(fileName);
        //Console.WriteLine("Слов: " + len);
        string[] allWords = new string[len];
        StreamReader sr = new StreamReader(fileName);
        string word = sr.ReadLine()!;
        int i = 0;
        while (word != null)
        {
            allWords[i] = word;
            i++;
            word = sr.ReadLine()!;
        }
        sr.Close();
        return allWords;
    }


    // подсчёт количества строк (слов) в файле
    private static int CountLines(string fileName)
    {
        int count = 0;
        StreamReader sr = new StreamReader(fileName);
        string word = sr.ReadLine()!;
        while (word != null)
        {
            count++;
            word = sr.ReadLine()!;
        }
        sr.Close();
        return count;
    }

    //все файлики с 5ти буквенными словами собираем в 1
    private static void AllToOneFile()
    {
        string[] files = new string[] { "5а.txt", "5б.txt", "5в.txt", "5г.txt", "5д.txt", "5е.txt", "5ж.txt", "5з.txt", "5и.txt", "5к.txt", "5л.txt", "5м.txt", "5н.txt", "5о.txt", "5п.txt", "5р.txt", "5с.txt", "5т.txt", "5у.txt", "5ф.txt", "5х.txt", "5ц.txt", "5ч.txt", "5ш.txt", "5щ.txt", "5э.txt", "5ю.txt", "5я.txt" };
        string dir = Directory.GetCurrentDirectory();
        //string readFile;
        string writeFile = "5letters.txt";
        StreamReader sr;
        string word;
        int count = 0;//, dif =0;
        StreamWriter sw = new StreamWriter(dir + "/" + writeFile);
        foreach (string name in files)
        {
            sr = new StreamReader(dir + "/" + name);
            word = sr.ReadLine()!;
            while (word != null)
            {
                sw.WriteLine(word);
                word = sr.ReadLine()!;
                count++;
            }
            //Console.WriteLine(name + ": " + (count - dif));
            //dif = count;
            sr.Close();
        }
        sw.Close();
        Console.WriteLine("Всего записано {0} слов", count);
    }


    // выбираем из файла только 5ти буквенные слова и сохр их в др файл
    private static void SelectFivelettersWords(string readFile, string writeFile)
    {
        string dir = Directory.GetCurrentDirectory();
        //Console.WriteLine(dir);
        //string readFile = "букваГ.txt";
        //string writeFile = "5г.txt";
        Console.WriteLine("Читаем \"{0}\"", readFile);
        Console.WriteLine("Пишем в \"{0}\"", writeFile);
        StreamReader sr;
        StreamWriter sw;

        try
        {
            sr = new StreamReader(dir + "/" + readFile);
        }
        catch
        {
            Console.WriteLine(readFile + " не открылся");
            return;
        }
        try
        {
            sw = new StreamWriter(dir + "/" + writeFile);
        }
        catch
        {
            Console.WriteLine("Не могу создать " + writeFile);
            return;
        }

        string line = sr.ReadLine()!;
        int rLines = 0, wLines = 0;
        while (line != null)
        {
            rLines++;
            if (line.Length == 5)
            {
                //Console.WriteLine(line);
                sw.WriteLine(line);
                wLines++;
            }
            line = sr.ReadLine()!;
        }
        sr.Close();
        sw.Close();
        Console.WriteLine("Считано {0} строк. Записано {1} строк.", rLines, wLines);
    }
}
