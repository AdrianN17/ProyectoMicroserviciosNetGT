namespace WalletService.Domain.Common;

public class Wallet : AggreateRoot<WalletId>
{
    public string Name { get; private set; } = default!; //obligatorio
    public string LastName { get; private set; } = default!;  //obligatorio
    public string FullName => $"{Name} {LastName}".Trim();  //obligatorio
    public Email Email { get; private set; } //obligatorio
    public PhoneNumber Phone { get; private set; } //obligatorio
    public WalletLimit Limit { get; private set; } //obligatorio
    
    public DocumentId Document { get; private set; } //obligatori
    
    public WalletStatus Status  { get; set; }

    private Wallet()
    {
    }
    
    public static Wallet Create(string name, string lastName, DocumentType documentType, string documentNumber, 
        string email, string phone, CurrencyType currency, decimal dailyLimit, WalletLimitId? walletLimitId = null)
    {
        // Validaciones básicas de campos requeridos
        var errors = ValidateFieldsRequired(name, lastName, documentType, documentNumber, email, phone, currency, dailyLimit);

        if (errors.Any())
            throw new ValidationException(errors);

        var wallet = new Wallet
        {
            Id = new WalletId(documentNumber),
            Name = name.Trim(),
            LastName = lastName.Trim(),
            Email = Email.Create(email),
            Phone = PhoneNumber.Create(phone),
            Limit = WalletLimit.Create(currency, dailyLimit, walletLimitId),
            Document = DocumentId.Create(documentType, documentNumber),
            Status = WalletStatus.OPERATIVE
        };

        return wallet;
    }

    private static Dictionary<string, string[]> ValidateFieldsRequired(string name, string lastName, DocumentType documentType, 
        string documentNumber, string email, string phone, CurrencyType currency, decimal dailyLimit)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
            errors["name"] = new[] { "El nombre es requerido." };

        if (string.IsNullOrWhiteSpace(lastName))
            errors["lastName"] = new[] { "El apellido es requerido." };

        // Validar documentType (enum) y documentNumber
        if (!Enum.IsDefined(typeof(DocumentType), documentType) || documentType.Equals(default(DocumentType)))
            errors["documentType"] = new[] { "El tipo de documento es requerido y debe ser válido." };

        if (string.IsNullOrWhiteSpace(documentNumber))
            errors["documentNumber"] = new[] { "El número de documento es requerido." };

        // Validación de email
        if (string.IsNullOrWhiteSpace(email))
            errors["email"] = new[] { "El email es requerido." };

        // Validación de phone
        if (string.IsNullOrWhiteSpace(phone))
            errors["phone"] = new[] { "El teléfono es requerido." };
        
        
        // Validación de límite / moneda
        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            errors["limit.currency"] = new[] { "El tipo de moneda es requerido y debe ser válido." };

        if (dailyLimit <= 0m)
            errors["limit.dailyLimit"] = new[] { "El límite diario debe ser mayor a 0." };
        
        return errors;
    }
    
    public void ChangeName(string name, string modifiedBy)
    {
        EnsureOperative();
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleViolationException("customer.name.required", "El nombre es obligatorio");
        Name = name.Trim();
        SetModified(modifiedBy);
    }

    public void ChangeLastName(string lastName, string modifiedBy)
    {
        EnsureOperative();
        if (string.IsNullOrWhiteSpace(lastName)) throw new BusinessRuleViolationException("customer.lastName.required", "El nombre es obligatorio");
        LastName = lastName.Trim();
        SetModified(modifiedBy);
    }
    
    public void ChangeDocument(DocumentType type, string number, string modifiedBy)
    {
        EnsureOperative();
        Document = DocumentId.Create(type, number);
        SetModified(modifiedBy);
    }
    
    public void ChangeEmail(string email, string modifiedBy)
    {
        EnsureOperative();
        var emailNew = Email.Create(email);
        if (emailNew.Equals(Email)) return; 
        Email = emailNew;
        SetModified(modifiedBy);
    }

    public void ChangePhone(string phone, string modifiedBy)
    {
        EnsureOperative();
        Phone = new PhoneNumber(phone);
        SetModified(modifiedBy);
    }

    public void ChangeLimit(CurrencyType currency, decimal dailyLimit, string modifiedBy)
    {
        EnsureOperative();
        
        if (dailyLimit <= 0) throw new BusinessRuleViolationException("wallet.limit.invalid", "El límite debe ser un valor positivo.");
        if ((dailyLimit % 500m) != 0m) throw new BusinessRuleViolationException("wallet.limit.invalid", "El límite debe ser múltiplo de 500.");

        Limit = WalletLimit.Create(currency, dailyLimit, Limit.WalletLimitId);
        SetModified(modifiedBy);
    }
    
    private void EnsureOperative()
    {
        if (Status != WalletStatus.OPERATIVE)
            throw new InvalidDomainStateException("wallet.status.suspended", "La billetera no está activo para esta operación.");
    }
    
    public void Operative(string modifiedBy)
    {
        if (Status == WalletStatus.OPERATIVE) return;
        Status = WalletStatus.OPERATIVE;
        SetModified(modifiedBy);
    }

    public void Duspended(string modifiedBy)
    {
        if (Status != WalletStatus.OPERATIVE)
            throw new InvalidDomainStateException("wallet.status.suspended", "Solo un cliente activo puede desactivarse.");
        Status = WalletStatus.SUSPENDED;
        SetModified(modifiedBy);
    }
}