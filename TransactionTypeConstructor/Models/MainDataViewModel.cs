namespace TransactionTypeConstructor.Models
{
    public class MainDataViewModel
    {
        public List<ConstructorData> ConstructorDatas { get; set; } 
        public ConstructorData ConstructorData { get; set; } 
        public List<AccountType> AccountTypes { get; set; } 
        public List<BalanceInfo> BalanceInfos { get; set; } 
        public List<SendType> SendTypes { get; set; } 
        public List<Currency> Currencies { get; set; }
        public List<SpkData> SpkDatas { get; set; }
        public List<TransactionType> TransactionTypes { get; set; }
        public TransactionType TransactionType { get; set; }

        public MainDataViewModel()
        {
            ConstructorDatas = new List<ConstructorData>();
            ConstructorData = new ConstructorData();
            AccountTypes = new List<AccountType>();
            BalanceInfos = new List<BalanceInfo>();
            SendTypes = new List<SendType>();
            Currencies = new List<Currency>();
            SpkDatas = new List<SpkData>();
            TransactionTypes = new List<TransactionType>();
            TransactionType = new TransactionType();
        }
    }
}
