
namespace ConsoleOrderExecutor.ConsoleFunction.Utils
{
    public interface IConsoleUtils
    {
        public bool GetParameter(string text, Predicate<string?> check, out string? result);
    }
    public class ConsoleUtils : IConsoleUtils
    {
        /// <summary>
        /// Try to receive value from console and return it in out parameter.
        /// </summary>
        /// <param name="text">Text describing what kind of parameter user should type?</param>
        /// <param name="check">Predicate that will check if value is correct.</param>
        /// <param name="result">Out parameter holding received value. If exist was typed it will be null.</param>
        /// <returns>True if user passed value or false if user want to exit procedure.</returns>
        public bool GetParameter(string text, Predicate<string?> check, out string? result)
        {
            Console.WriteLine(text);
            while (true)
            {
                string? value = Console.ReadLine();
                if ((value ?? "") == "exit")
                {
                    result = null;
                    return false;
                }
                if (check(value))
                {
                    result = value;
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid value.");
                }
            }
        }
    }
}
