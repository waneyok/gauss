using System;
using System.IO;
using System.Linq;
using System.Globalization;

class Program
{
    static void Main()
    {
        string inputFile = "input.txt";
        string outputFile = "output.txt";

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Ошибка: Файл '{inputFile}' не найден!");
            Console.WriteLine("Пожалуйста, создайте файл input.txt с коэффициентами матрицы.");
            return;
        }

        try
        {
            // 1. Чтение данных из файла
            string[] lines = File.ReadAllLines(inputFile)
                                 .Where(line => !string.IsNullOrWhiteSpace(line))
                                 .ToArray();

            int n = lines.Length; // Количество уравнений (неизвестных)
            double[,] matrix = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                // Разделяем строку по пробелам или табуляциям
                string[] parts = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != n + 1)
                {
                    Console.WriteLine($"Ошибка в строке {i + 1}: ожидалось {n + 1} чисел, а найдено {parts.Length}.");
                    return;
                }

                for (int j = 0; j <= n; j++)
                {
                    // Заменяем точку/запятую для универсального парсинга
                    string numStr = parts[j].Replace(',', '.');
                    matrix[i, j] = double.Parse(numStr, CultureInfo.InvariantCulture);
                }
            }

            // 2. Прямой ход метода Гаусса
            for (int i = 0; i < n - 1; i++)
            {
                // Выбор главного элемента (перестановка строк при делении на 0)
                if (Math.Abs(matrix[i, i]) < 1e-9)
                {
                    int maxRow = i;
                    for (int k = i + 1; k < n; k++)
                    {
                        if (Math.Abs(matrix[k, i]) > Math.Abs(matrix[maxRow, i]))
                            maxRow = k;
                    }

                    for (int j = 0; j <= n; j++)
                    {
                        double temp = matrix[i, j];
                        matrix[i, j] = matrix[maxRow, j];
                        matrix[maxRow, j] = temp;
                    }
                }

                // Исключение переменной x_i
                for (int k = i + 1; k < n; k++)
                {
                    double factor = matrix[k, i] / matrix[i, i];
                    for (int j = i; j <= n; j++)
                    {
                        matrix[k, j] -= factor * matrix[i, j];
                    }
                }
            }

            // 3. Обратный ход метода Гаусса
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += matrix[i, j] * x[j];
                }
                x[i] = (matrix[i, n] - sum) / matrix[i, i];
            }

            // 4. Запись результатов в файл output.txt
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("--- Решение СЛАУ методом Гаусса ---");
                writer.WriteLine($"Количество уравнений: {n}\n");
                writer.WriteLine("Найденные неизвестные:");

                for (int i = 0; i < n; i++)
                {
                    writer.WriteLine($"x[{i + 1}] = {x[i]:F4}");
                }
            }

            Console.WriteLine($"Расчет успешно выполнен!");
            Console.WriteLine($"Результаты сохранены в файл: {Path.GetFullPath(outputFile)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при обработке: {ex.Message}");
        }
    }
}
