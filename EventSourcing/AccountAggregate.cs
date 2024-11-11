using EventSourcing.Events;
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
    foreach (var accountEvent in events)
    {
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
    throw new NotImplementedException();
  }

  private void Apply(ClosureEvent closure)
  {
    throw new NotImplementedException();
  }
}
