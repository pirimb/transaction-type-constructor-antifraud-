using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using TransactionTypeConstructor.Interfaces;
using TransactionTypeConstructor.Models;

namespace TransactionTypeConstructor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static IDB db;


        public HomeController(IDB _db, ILogger<HomeController> logger)
        {
            db = _db;
            _logger = logger;
        }

        [Authorize(Roles = "Basic")]
        public IActionResult Index()
        {
            var constructorDatas = new MainDataViewModel();
            constructorDatas.ConstructorDatas = db.GetConstructorData();
            return View(constructorDatas);
        }

        [HttpGet]
        public IActionResult EditData(int IdForEdit)
         {
            var constructorData = new MainDataViewModel();
            constructorData.ConstructorData = db.GetConstructorDataById(IdForEdit);
            constructorData.AccountTypes = db.GetAccountTypes();
            constructorData.BalanceInfos = db.GetBalanceInfo();
            constructorData.SendTypes = db.GetSendTypes();
            constructorData.Currencies = db.GetCurrencies();
            constructorData.SpkDatas = db.GetSpkDatas();
            constructorData.TransactionTypes = db.GetTransactionTypes();
            return View(constructorData);
        }
        
        [HttpPost]
        public IActionResult EditData(MainDataViewModel model)
        {
            db.UpdateData(model.ConstructorData);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult AddNewData()
        {
            var constructorData = new MainDataViewModel();
            constructorData.AccountTypes = db.GetAccountTypes();
            constructorData.BalanceInfos = db.GetBalanceInfo();
            constructorData.SendTypes = db.GetSendTypes();
            constructorData.Currencies = db.GetCurrencies();
            constructorData.SpkDatas = db.GetSpkDatas();
            constructorData.TransactionTypes = db.GetTransactionTypes();
            return View("NewData" , constructorData);
        }

        [HttpPost]
        public IActionResult AddNewData(MainDataViewModel model)
        {
            db.AddNewData(model.ConstructorData);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteData(int idForDelete)
        {
            db.DeleteData(idForDelete);
            return RedirectToAction("Index", "Home");
        }
        

        public IActionResult TrnTypes()
        {
            var transactionTypes = new MainDataViewModel();
            transactionTypes.TransactionTypes = db.GetTransactionTypes();
            return View("TrnTypes", transactionTypes);
        }

        public IActionResult AddNewTrnType(string trnInput_description)
        {
            db.AddNewTrnType(trnInput_description);
            return RedirectToAction("TrnTypes", "Home");
        }
        
        [HttpGet]
        public IActionResult EditTrnType(int IdForEditTrn)
        {
            var transactionType = new MainDataViewModel().TransactionType;
            transactionType = db.GetTrnTypeById(IdForEditTrn);
            return View("EditTrnType", transactionType);
        }
        
        [HttpPost]
        public IActionResult EditTrnType(TransactionType model)
        {
            db.UpdateTrnType(model);
            return RedirectToAction("TrnTypes", "Home");
        }
        public IActionResult DeleteTrnType(int idForDeleteTrn)
        {
            db.DeleteTrnType(idForDeleteTrn);
            return RedirectToAction("TrnTypes", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public int CheckAccount(string account)
        {
            var result = db.GetCorrectHesab(account);
            return result;
        }
        public int CheckOwner(string ownerNumber)
        {
            var result = db.GetCorrectOwner(ownerNumber);
            return result;
        }
        public string GetUserNameFromSession()
        {
            var obj = new LoginModel();
            try
            {
                var str = HttpContext.Session.GetString("user");

                obj = JsonConvert.DeserializeObject<LoginModel>(str);
            }
            catch (Exception e)
            {
                RedirectToAction("Logout", "Account");
            }
            return obj.UserName;
        }

        public string GetTrnType(long trnType)
        {
            var description = db.GetTrnType(trnType);
            return description;
        }

    }

}