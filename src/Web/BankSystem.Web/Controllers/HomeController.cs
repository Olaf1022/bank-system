﻿namespace BankSystem.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Areas.MoneyTransfers.Models;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.BankAccount;
    using Services.Interfaces;
    using Services.Models.BankAccount;
    using Services.Models.MoneyTransfer;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IMoneyTransferService moneyTransferService;
        private readonly IUserService userService;

        public HomeController(
            IBankAccountService bankAccountService,
            IUserService userService,
            IMoneyTransferService moneyTransferService)
        {
            this.bankAccountService = bankAccountService;
            this.userService = userService;
            this.moneyTransferService = moneyTransferService;
        }


        public async Task<IActionResult> Index()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.View("IndexGuest");
            }

            var userId = await this.userService.GetUserIdByUsernameAsync(this.User.Identity.Name);
            var bankAccounts =
                (await this.bankAccountService.GetAllAccountsByUserIdAsync<BankAccountIndexServiceModel>(userId))
                .Select(Mapper.Map<BankAccountIndexViewModel>)
                .ToArray();
            var moneyTransfers = (await this.moneyTransferService
                    .GetLast10MoneyTransfersForUserAsync<MoneyTransferListingServiceModel>(userId))
                .Select(Mapper.Map<MoneyTransferListingDto>)
                .ToArray();

            var viewModel = new HomeViewModel
            {
                UserBankAccounts = bankAccounts,
                MoneyTransfers = moneyTransfers
            };

            return this.View(viewModel);
        }
    }
}