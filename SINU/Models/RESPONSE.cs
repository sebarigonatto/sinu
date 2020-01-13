
// NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class RESPONSE
{

    private RESPONSEROW[] dISTRICTField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ROW", IsNullable = false)]
    public RESPONSEROW[] DISTRICT
    {
        get
        {
            return this.dISTRICTField;
        }
        set
        {
            this.dISTRICTField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class RESPONSEROW
{

    private string pROVINCEField;

    private string dESCRIPTIONField;

    private byte iNDEXField;

    /// <remarks/>
    public string PROVINCE
    {
        get
        {
            return this.pROVINCEField;
        }
        set
        {
            this.pROVINCEField = value;
        }
    }

    /// <remarks/>
    public string DESCRIPTION
    {
        get
        {
            return this.dESCRIPTIONField;
        }
        set
        {
            this.dESCRIPTIONField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte INDEX
    {
        get
        {
            return this.iNDEXField;
        }
        set
        {
            this.iNDEXField = value;
        }
    }
}


