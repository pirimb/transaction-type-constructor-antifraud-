using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using TransactionTypeConstructor.Interfaces;
using TransactionTypeConstructor.Models;

namespace TransactionTypeConstructor.Repository
{
    public class DB : IDB
    {
        private static string connectionString;
        private static DbConfig DBConfig;
        private static ILogger<DB> log;

        public DB(IOptions<DbConfig> options, ILogger<DB> _log)
        {
            DBConfig = options.Value;
            log = _log;
        }

        public void Init(LoginModel user)
        {
            connectionString = $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={DBConfig.Host})(PORT={DBConfig.Port})))" +
                                $"(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={DBConfig.ServiceName}))); " +
                                $"User Id ={user.UserName}; Password ={user.Password}; Min Pool Size = 1; Max Pool Size = 10; Pooling = True;" +
                                $" Validate Connection = true; Connection Lifetime = 300; Connection Timeout = 300;";
        }

        public OracleConnection GetConnection()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string not initialized!");
            }
            var connection = new OracleConnection(connectionString);
            connection.Open();
            return connection;
        }

        public int TestConnection()
        {
            int res = -1;
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT 1 FROM DUAL";
                        cmd.CommandType = CommandType.Text;

                        cmd.ExecuteNonQuery();
                        res = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return res;
        }

        public int GetAccessLevel(LoginModel model)
        {
            int res = -1;
            OracleConnection con = null;
            if (model.UserName.IndexOf("bnk_") == 0)
            {
                model.UserName = model.UserName.Remove(0, 4);
            }
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT anti_fraud_admin FROM SBNK_PRL.CAV_ICR WHERE ICR_AD = '{model.UserName}'";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res = Convert.ToInt32(reader["anti_fraud_admin"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Test Connection: User - {model.UserName} Error - {ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
            return res;
        }

        public ConstructorData GetConstructorDataById(int id)
        {
            var constructorData = new ConstructorData();
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"SBNK_PRL.PKG_ANTI_FRAUD.GET_CONSTRUCTOR_DATA";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input).Value = id;
                        cmd.Parameters.Add("P_LIST", OracleDbType.RefCursor, ParameterDirection.Output).Size = 100000;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                constructorData.CHK_ONLINE = reader["CHK_ONLINE"].ToString();
                                constructorData.CR_ACCOUNT = reader["CR_ACCOUNT"].ToString(); ;
                                constructorData.CR_BAL = reader["CR_BAL"].ToString();
                                constructorData.CR_CURRENCY = reader["CR_CURRENCY"].ToString();
                                constructorData.CR_HES_NOV = reader["CR_HES_NOV"].ToString();
                                constructorData.CR_OWNER = reader["CR_OWNER"].ToString();
                                constructorData.DB_ACCOUNT = reader["DB_ACCOUNT"].ToString();
                                constructorData.DB_BAL = reader["DB_BAL"].ToString();
                                constructorData.DB_CURRENCY = reader["DB_CURRENCY"].ToString();
                                constructorData.DB_HES_NOV = reader["DB_HES_NOV"].ToString();
                                constructorData.DB_OWNER = reader["DB_OWNER"].ToString();
                                constructorData.DESCRIPTION = reader["DESCRIPTION"].ToString();
                                constructorData.G_NOV = reader["G_NOV"].ToString();
                                constructorData.ID = Convert.ToInt32(reader["ID"]);
                                constructorData.SPK_SYSTEM = reader["SPK_SYSTEM"].ToString();
                                constructorData.SPK_TR_TYPE = reader["SPK_TR_TYPE"].ToString();
                                constructorData.TRN_TYPE = Convert.ToInt32(reader["TRN_TYPE"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return constructorData;
        }


        public void AddNewData(ConstructorData model)
        {
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SBNK_PRL.PKG_ANTI_FRAUD.INSERT_CONSTRUCTOR_DATA";
                        cmd.CommandType = CommandType.StoredProcedure;

                        //cmd.Parameters.Add("sbnk_prl.ANTI_FRAUD_RULES.ID", OracleDbType.Int32, ParameterDirection.Input).Value = model.ID;
                        cmd.Parameters.Add("p_G_NOV", OracleDbType.Int32, ParameterDirection.Input).Value = model.G_NOV;
                        cmd.Parameters.Add("p_DB_HES_NOV", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_HES_NOV;
                        cmd.Parameters.Add("p_DB_BAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_BAL;
                        cmd.Parameters.Add("p_DB_OWNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_OWNER;
                        cmd.Parameters.Add("p_DB_CURRENCY", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_CURRENCY;
                        cmd.Parameters.Add("p_DB_ACCOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_ACCOUNT;
                        cmd.Parameters.Add("p_CR_HES_NOV", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_HES_NOV;
                        cmd.Parameters.Add("p_CR_BAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_BAL;
                        cmd.Parameters.Add("p_CR_OWNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_OWNER;
                        cmd.Parameters.Add("p_CR_CURRENCY", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_CURRENCY;
                        cmd.Parameters.Add("p_CR_ACCOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_ACCOUNT;
                        cmd.Parameters.Add("p_SPK_SYSTEM", OracleDbType.Int32, ParameterDirection.Input).Value = model.SPK_SYSTEM;
                        cmd.Parameters.Add("p_SPK_TR_TYPE", OracleDbType.Int32, ParameterDirection.Input).Value = model.SPK_TR_TYPE;
                        cmd.Parameters.Add("p_TRN_TYPE", OracleDbType.Int32, ParameterDirection.Input).Value = model.TRN_TYPE;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }

        public void UpdateData(ConstructorData model)
        {
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SBNK_PRL.PKG_ANTI_FRAUD.UPDATE_CONSTRUCTOR_DATA";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_ID", OracleDbType.Int32, ParameterDirection.Input).Value = model.ID;
                        cmd.Parameters.Add("p_G_NOV", OracleDbType.Int32, ParameterDirection.Input).Value = model.G_NOV;
                        cmd.Parameters.Add("p_DB_HES_NOV", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_HES_NOV;
                        cmd.Parameters.Add("p_DB_BAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_BAL;
                        cmd.Parameters.Add("p_DB_OWNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_OWNER;
                        cmd.Parameters.Add("p_DB_CURRENCY", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_CURRENCY;
                        cmd.Parameters.Add("p_DB_ACCOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DB_ACCOUNT;
                        cmd.Parameters.Add("p_CR_HES_NOV", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_HES_NOV;
                        cmd.Parameters.Add("p_CR_BAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_BAL;
                        cmd.Parameters.Add("p_CR_OWNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_OWNER;
                        cmd.Parameters.Add("p_CR_CURRENCY", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_CURRENCY;
                        cmd.Parameters.Add("p_CR_ACCOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.CR_ACCOUNT;
                        cmd.Parameters.Add("p_SPK_SYSTEM", OracleDbType.Int32, ParameterDirection.Input).Value = model.SPK_SYSTEM;
                        cmd.Parameters.Add("p_SPK_TR_TYPE", OracleDbType.Int32, ParameterDirection.Input).Value = model.SPK_TR_TYPE;
                        cmd.Parameters.Add("p_TRN_TYPE", OracleDbType.Int32, ParameterDirection.Input).Value = model.TRN_TYPE;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }

        public void DeleteData(int idForDelete)
        {
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"SBNK_PRL.PKG_ANTI_FRAUD.DELETE_CONSTRUCTOR_DATA";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input).Value = idForDelete;


                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }

        public List<ConstructorData> GetConstructorData()
        {
            var constructorDatas = new List<ConstructorData>();
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"SBNK_PRL.PKG_ANTI_FRAUD.GET_CONSTRUCTOR_DATA";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_ID", OracleDbType.Int32, ParameterDirection.Input).Value = null;
                        cmd.Parameters.Add("P_LIST", OracleDbType.RefCursor, ParameterDirection.Output).Size = 100000;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ConstructorData constructorData = new ConstructorData();

                                constructorData.CHK_ONLINE = reader["CHK_ONLINE"].ToString();
                                constructorData.CR_ACCOUNT = reader["CR_ACCOUNT"].ToString(); ;
                                constructorData.CR_BAL = reader["CR_BAL"].ToString();
                                constructorData.CR_CURRENCY = reader["CR_CURRENCY"].ToString();
                                constructorData.CR_HES_NOV = reader["CR_HES_NOV"].ToString();
                                constructorData.CR_OWNER = reader["CR_OWNER"].ToString();
                                constructorData.DB_ACCOUNT = reader["DB_ACCOUNT"].ToString();
                                constructorData.DB_BAL = reader["DB_BAL"].ToString();
                                constructorData.DB_CURRENCY = reader["DB_CURRENCY"].ToString();
                                constructorData.DB_HES_NOV = reader["DB_HES_NOV"].ToString();
                                constructorData.DB_OWNER = reader["DB_OWNER"].ToString();
                                constructorData.DESCRIPTION = reader["DESCRIPTION"].ToString();
                                constructorData.G_NOV = reader["G_NOV"].ToString();
                                constructorData.ID = Convert.ToInt32(reader["ID"]);
                                constructorData.SPK_SYSTEM = reader["SPK_SYSTEM"].ToString();
                                constructorData.SPK_TR_TYPE = reader["SPK_TR_TYPE"].ToString();
                                constructorData.TRN_TYPE = Convert.ToInt32(reader["TRN_TYPE"]);

                                constructorDatas.Add(constructorData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return constructorDatas;
        }

        public int GetCorrectHesab(string hesab)
        {
            OracleConnection con = null;
            int isCorrectHesab = 0;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"select count(*) cnt from sbnk_prl.hesablar t where t.HESAB = '{hesab}'";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                isCorrectHesab = Convert.ToInt32(reader["cnt"].ToString());
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isCorrectHesab;
        }


        public int GetCorrectOwner(string owner)
        {
            OracleConnection con = null;
            int isCorrectOwner = 0;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"select count(*) cnt from sbnk_prl.mushteri t where t.m_qeydn = '{owner}'";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                isCorrectOwner = Convert.ToInt32(reader["cnt"].ToString());
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return isCorrectOwner;
        }

        public string GetTrnType(long id)
        {
            OracleConnection con = null;
            int trnType = 0;
            string trnDesc = "";

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SBNK_PRL.PKG_ANTI_FRAUD.GET_TRN_TYPE";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("res", OracleDbType.Int32, ParameterDirection.ReturnValue);
                        cmd.Parameters.Add("P_IDN", OracleDbType.Decimal, ParameterDirection.Input).Value = id;

                        cmd.ExecuteNonQuery();
                        if (!string.IsNullOrEmpty(cmd.Parameters["res"].Value.ToString()) && cmd.Parameters["res"].Value.ToString() != "null")
                        {
                            trnType = Convert.ToInt32(cmd.Parameters["res"].Value.ToString());

                            cmd.Parameters.Clear();

                            cmd.CommandText = $"select t.DESCRIPTION from sbnk_prl.ANTI_FRAUD_TRN_TYPES t where t.id = '{trnType}'";
                            cmd.CommandType = CommandType.Text;

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    trnDesc = reader["DESCRIPTION"].ToString();
                                }
                            }
                        }
                        else
                        {
                            trnDesc = "Daxil edilən ödəniş id-si yanlışdır!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
                trnDesc = ex.Message;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return trnDesc;
        }

       
        public void AddNewTrnType(string trnInput_description)
        {
            OracleConnection con = null;
            if (trnInput_description != null && trnInput_description != "null")
            {
                try
                {
                    using (con = GetConnection())
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "sbnk_prl.insert_newTrn_data";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("p_DESCRIPTION", OracleDbType.Varchar2, ParameterDirection.Input).Value = trnInput_description;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"{ex.Message}");
                }
                finally
                {
                    if (con != null)
                    {
                        con.Close();
                        con.Dispose();

                    }
                }
            }
        }

        public TransactionType GetTrnTypeById(int id)
        {
            OracleConnection con = null;
            var transactionType = new TransactionType();

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"select * from sbnk_prl.ANTI_FRAUD_TRN_TYPES t where t.id = '{id}'";

                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                transactionType.ID = Convert.ToInt32(reader["ID"].ToString());
                                transactionType.DESCRIPTION = reader["DESCRIPTION"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
            return transactionType;
        }

        public void UpdateTrnType(TransactionType model)
        {
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SBNK_PRL.update_trn_type";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_ID", OracleDbType.Int32, ParameterDirection.Input).Value = model.ID;
                        cmd.Parameters.Add("p_DESCRIPTION", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.DESCRIPTION;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }

        public void DeleteTrnType(int id)
        {
            OracleConnection con = null;
            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = $"DELETE from sbnk_prl.ANTI_FRAUD_TRN_TYPES t where t.id = '{id}'";
                        cmd.CommandType = CommandType.Text;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }
        }


        //----------Data for dropDown boxes in edit and insert pages

        public List<AccountType> GetAccountTypes()
        {
            var accountTypes = new List<AccountType>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select t.HES_NOV, t.NOV_AD from sbnk_prl.hes_novler t";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccountType accountType = new AccountType();

                                accountType.HES_NOV = reader["HES_NOV"].ToString();
                                accountType.NOV_AD = reader["NOV_AD"].ToString();

                                accountTypes.Add(accountType);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }


            return accountTypes;
        }

        public List<BalanceInfo> GetBalanceInfo()
        {
            var balanceInfos = new List<BalanceInfo>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select t.KOD_BAL, t.BAL_AD from bal_hes1 t";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var balanceInfo = new BalanceInfo();

                                balanceInfo.KOD_BAL = reader["KOD_BAL"].ToString();
                                balanceInfo.BAL_AD = reader["BAL_AD"].ToString();

                                balanceInfos.Add(balanceInfo);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }


            return balanceInfos;
        }

        public List<SendType> GetSendTypes()
        {
            var sendTypes = new List<SendType>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select * from sbnk_prl.gonderme_novler";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var sendType = new SendType();

                                sendType.ID = reader["ID"].ToString();
                                sendType.NAME = reader["NAME"].ToString();

                                sendTypes.Add(sendType);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }

            return sendTypes;
        }


        public List<Currency> GetCurrencies()
        {
            var curencies = new List<Currency>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select t.VALQ_AD from sbnk_prl.valyuta t";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var currency = new Currency();

                                currency.VALQ_AD = reader["VALQ_AD"].ToString();

                                curencies.Add(currency);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }

            return curencies;
        }


        public List<SpkData> GetSpkDatas()
        {
            var spkDatas = new List<SpkData>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select * from sbnk_prl.spk_list t where t.status = '1'";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var spkData = new SpkData();

                                spkData.KOD = reader["KOD"].ToString();
                                spkData.ADI = reader["ADI"].ToString();

                                spkDatas.Add(spkData);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }

            return spkDatas;
        }

        public List<TransactionType> GetTransactionTypes()
        {
            var transactionTypes = new List<TransactionType>();
            OracleConnection con = null;

            try
            {
                using (con = GetConnection())
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "select * from sbnk_prl.ANTI_FRAUD_TRN_TYPES";
                        cmd.CommandType = CommandType.Text;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var transactionType = new TransactionType();

                                transactionType.ID = Convert.ToInt32(reader["ID"].ToString());
                                transactionType.DESCRIPTION = reader["DESCRIPTION"].ToString();

                                transactionTypes.Add(transactionType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                log.LogError($"{ex.Message}");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();

                }
            }

            return transactionTypes;
        }

        //-----------------------

    }
}
