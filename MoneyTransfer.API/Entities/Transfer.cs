﻿namespace MoneyTransfer.API.Entities;

public partial class Transfer
{
    public int Id { get; set; }

    public int TransferTypeId { get; set; }

    public int TransferStatusId { get; set; }

    public int AccountIdFrom { get; set; }

    public int AccountIdTo { get; set; }

    public decimal Amount { get; set; }

    public DateOnly DateCreated { get; set; }

    public virtual Account AccountIdFromNavigation { get; set; } = null!;

    public virtual Account AccountIdToNavigation { get; set; } = null!;

    public TransferStatus TransferStatus => (TransferStatus)TransferStatusId;
    
    public TransferType TransferType => (TransferType)TransferTypeId;

    public static Transfer NotFound => new() { Id = 0, TransferTypeId = 0, TransferStatusId = 0, AccountIdFromNavigation = null!, AccountIdToNavigation = null!, AccountIdFrom = 0, AccountIdTo = 0, Amount = 0, DateCreated = DateOnly.MinValue };
}
