
namespace GICApplication
{
    public interface IGICBankRepository
    {
        void AddTransactions(InputTransaction transaction);
        void AddInterestRules(InputRule rule);
        void PrintStatement(InputPrint print);
    }
}
