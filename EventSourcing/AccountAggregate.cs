﻿using EventSourcing.Events;
using EventSourcing.Exceptions;
using EventSourcing.Models;

namespace EventSourcing;

public class AccountAggregate
{

  public string? AccountId { get; set; }
  public decimal Balance { get; set; }
  public decimal MaxBalance { get; set; }
  public CurrencyType Currency { get; set; }
  public string? CustomerId { get; set; }
  public AccountStatus Status { get; set; }
  public List<LogMessage>? AccountLog { get; set; }

  private AccountAggregate(){}

  public static AccountAggregate? GenerateAggregate(Event[] events)
  {
    if (events.Length == 0)
    {
      return null;
    }
    
    var account = new AccountAggregate();
    int currentEventId = 1; 
    foreach (var accountEvent in events)
    {
      if(accountEvent.EventId != currentEventId)
      {
        throw new Exception("511 ERROR_INVALID_EVENT_STREAM");
      }
      currentEventId++;
      account.Apply(accountEvent);
    }

    return account;
  }

  private void Apply(Event accountEvent)
  {
    switch (accountEvent)
    {
      case AccountCreatedEvent accountCreated:
        Apply(accountCreated);
        break;
      case DepositEvent deposit:
        Apply(deposit);
        break;
      case WithdrawalEvent withdrawal:
        Apply(withdrawal);
        break;
      case DeactivationEvent deactivation:
        Apply(deactivation);
        break;
      case ActivationEvent activation:
        Apply(activation);
        break;
      case ClosureEvent closure:
        Apply(closure);
        break;
      case CurrencyChangeEvent currencyChange:
        Apply(currencyChange);
        break;
      default:
        throw new EventTypeNotSupportedException("162 ERROR_EVENT_NOT_SUPPORTED");
    }
  } 

  private void Apply(AccountCreatedEvent accountCreated)
  {
    AccountId = accountCreated.AccountId;
    Balance = accountCreated.InitialBalance;
    MaxBalance = accountCreated.MaxBalance;
    Currency = accountCreated.Currency;
    CustomerId = accountCreated.CustomerId;
  }

  private void Apply(DepositEvent deposit)
  {
    if(AccountId == null)
    {
      throw new Exception("128 ERROR_ACCOUNT_UNINSTANTIATED");
    }
    else if(Status == AccountStatus.Disabled)
    {
      throw new Exception("344 ERROR_TRANSACTION_REJECTED_ACCOUNT_DEACTIVATED");
    }
    else if(Status == AccountStatus.Closed)
    {
      throw new Exception("502 ERROR_ACCOUNT_CLOSED");
    }

    Balance += deposit.Amount;

    if(Balance > MaxBalance)
    {
      throw new Exception("281 ERROR_BALANCE_SUCCEED_MAX_BALANCE");
    }
  }

  private void Apply(WithdrawalEvent withdrawal)
  {
    if(AccountId == null)
    {
      throw new Exception("128 ERROR_ACCOUNT_UNINSTANTIATED");
    }
    else if(Status == AccountStatus.Disabled)
    {
      throw new Exception("344 ERROR_TRANSACTION_REJECTED_ACCOUNT_DEACTIVATED");
    }

    Balance -= withdrawal.amount;

    if(Balance < 0)
    {
      throw new Exception("285 ERROR_BALANCE_IN_NEGATIVE");
    }

  }

  private void Apply(DeactivationEvent deactivation)
  {
    Status = AccountStatus.Disabled;
    if(AccountLog == null)
    {
      AccountLog = new();
    }
    AccountLog.Add(new LogMessage(
      "DEACTIVATE", 
      deactivation.Reason, 
      deactivation.Timestamp
      ));
  }

  private void Apply(ActivationEvent activation)
  {
    if(Status == AccountStatus.Enabled)
    {
      return;
    }

    Status = AccountStatus.Enabled;

    if(AccountLog == null)
    {
      AccountLog = new();
    }
    AccountLog.Add(new LogMessage(
      "ACTIVATE", 
      "Account reactivated", 
      activation.Timestamp
      ));
  }

  private void Apply(CurrencyChangeEvent currencyChange)
  {
    if(AccountLog == null)
    {
      AccountLog = new();
    }
    AccountLog.Add(new LogMessage(
      "CURRENCY-CHANGE", 
      $"Change currency from '{Currency.ToString().ToUpper()}' to '{currencyChange.NewCurrency.ToString().ToUpper()}'", 
      currencyChange.Timestamp
      ));

    Currency = currencyChange.NewCurrency;
    Balance = currencyChange.NewBalance;
    Status = AccountStatus.Disabled;
    
  }

  private void Apply(ClosureEvent closure)
  {
    if(AccountLog == null)
    {
      AccountLog = new();
    }
    AccountLog.Add(new LogMessage(
      "CLOSURE", 
      $"Reason: {closure.Reason}, Closing Balance: '{Convert.ToInt32(Balance)}'", 
      closure.Timestamp
      ));
    Status = AccountStatus.Closed;
  }
}
