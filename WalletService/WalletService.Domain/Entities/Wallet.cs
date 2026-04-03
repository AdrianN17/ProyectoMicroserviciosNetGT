namespace WalletService.Domain.Common;

public class Wallet : AggreateRoot<WalletId>
{
    public string Name { get; private set; } = default!;
    public string LastName { get; private set; } = default!;  
    public Email Email { get; private set; } 
    public PhoneNumber Phone { get; private set; } 
    public WalletLimit Limit { get; private set; } 
    
    public DocumentId Document { get; private set; } 
    
    public WalletStatus Status  { get; private set; } 
    
    private WalletLimit WalletLimit { get; private set; } 

    private Wallet()
    {
    }
    
    public static Wallet Create(string name, string lastName, DocumentType documentType, string documentNumber, 
        string email, string phone, CurrencyType currency, decimal dailyLimit, )
    {
        var errors = ValidateFieldsRequired(name, lastName, documentType, documentNumber, email, phone);

        if (errors.Any())
            throw new ValidationException(errors);

        var wallet = new Wallet();
        try
        {

            var walletId = WalletId.NewId();

            var walletLimit = WalletLimit.Create(
                walletId,
                currency,
                dailyLimit
            );

            wallet = new Wallet
            {
                Id = walletId
                Name = name.Trim(),
                LastName = lastName.Trim(),
                Email = Email.Create(email),
                Phone = PhoneNumber.Create(phone),
                Limit = WalletLimit.Create(currency, dailyLimit, walletLimitId),
                Document = DocumentId.Create(documentType, documentNumber),
                Status = WalletStatus.OPERATIVE,
                WalletLimit = walletLimit
            };
        }
        catch (InvalidValueObjectException iv)
        {
            var prefix = iv.Code switch
            {
                "document.invalid" => "documentNumber",
                "currency.invalid" => "currency",
                _ => ""
            };

            var errorsVo = iv.Errors.ToDictionary(k => $"{prefix}", v => v.Value);

            //foreach (var kv in DomainErrors.Prefix($"{prefix}", iv.Errors))
            foreach (var kv in errorsVo)
                errors[kv.Key] = kv.Value;

        }

        if (errors.Count > 0)
                throw new DomainValidationException("wallet.invalid", "Domain validation failed", errors);

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
        
        return errors;
    }
    
    public void ChangeName(string name)
    {
        EnsureOperative();
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleViolationException("wallet.name.required", "El nombre es obligatorio");
        Name = name.Trim();
        SetModified();
    }

    public void ChangeLastName(string lastName)
    {
        EnsureOperative();
        if (string.IsNullOrWhiteSpace(lastName)) throw new BusinessRuleViolationException("wallet.lastName.required", "El nombre es obligatorio");
        LastName = lastName.Trim();
        SetModified();
    }
    
    public void ChangeDocument(DocumentType type, string number)
    {
        EnsureOperative();
        Document = DocumentId.Create(type, number);
        SetModified();
    }
    
    public void ChangeEmail(string email)
    {
        EnsureOperative();
        var emailNew = Email.Create(email);
        if (emailNew.Equals(Email)) return; 
        Email = emailNew;
        SetModified();
    }

    public void ChangePhone(string phone)
    {
        EnsureOperative();
        Phone = new PhoneNumber(phone);
        SetModified();
    }
    
    private void EnsureOperative()
    {
        if (Status != WalletStatus.OPERATIVE)
            throw new InvalidDomainStateException("wallet.status.suspended", "La billetera no está activo para esta operación.");
    }
    
    public void Operative()
    {
        if (Status == WalletStatus.OPERATIVE) return;
        Status = WalletStatus.OPERATIVE;
        SetModified();
    }

    public void Duspended()
    {
        if (Status != WalletStatus.OPERATIVE)
            throw new InvalidDomainStateException("wallet.status.suspended", "Solo un cliente activo puede desactivarse.");
        Status = WalletStatus.SUSPENDED;
        SetModified();
    }
}