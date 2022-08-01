using Oracle.ManagedDataAccess.Client;
using TransactionTypeConstructor.Models;

namespace TransactionTypeConstructor.Interfaces
{
    public interface IDB
    {
        public void Init(LoginModel model);
        public OracleConnection GetConnection();
        public int GetAccessLevel(LoginModel model);
        public int TestConnection();
        public List<ConstructorData> GetConstructorData();
        public ConstructorData GetConstructorDataById(int id);
        public void AddNewData(ConstructorData model);
        public void UpdateData(ConstructorData model);
        public void DeleteData(int idForDelete);
        public List<AccountType> GetAccountTypes();
        public List<BalanceInfo> GetBalanceInfo();
        public List<SendType> GetSendTypes();
        public List<Currency> GetCurrencies();
        public List<SpkData> GetSpkDatas();
        public List<TransactionType> GetTransactionTypes();
        public int GetCorrectHesab(string hesab);
        public int GetCorrectOwner(string owner);
        public string GetTrnType(long id);
        public void AddNewTrnType(string trnInput_description);
        public TransactionType GetTrnTypeById(int id);
        public void UpdateTrnType(TransactionType model);
        public void DeleteTrnType(int id);
    }
}
