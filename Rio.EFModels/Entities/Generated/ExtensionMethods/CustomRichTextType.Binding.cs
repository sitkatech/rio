//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomRichTextType]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Rio.Models.DataTransferObjects;


namespace Rio.EFModels.Entities
{
    public abstract partial class CustomRichTextType
    {
        public static readonly CustomRichTextTypeHomePage HomePage = Rio.EFModels.Entities.CustomRichTextTypeHomePage.Instance;
        public static readonly CustomRichTextTypeContact Contact = Rio.EFModels.Entities.CustomRichTextTypeContact.Instance;
        public static readonly CustomRichTextTypeFrequentlyAskedQuestions FrequentlyAskedQuestions = Rio.EFModels.Entities.CustomRichTextTypeFrequentlyAskedQuestions.Instance;
        public static readonly CustomRichTextTypeAboutGET AboutGET = Rio.EFModels.Entities.CustomRichTextTypeAboutGET.Instance;
        public static readonly CustomRichTextTypeDisclaimer Disclaimer = Rio.EFModels.Entities.CustomRichTextTypeDisclaimer.Instance;
        public static readonly CustomRichTextTypePlatformOverview PlatformOverview = Rio.EFModels.Entities.CustomRichTextTypePlatformOverview.Instance;
        public static readonly CustomRichTextTypeMeasuringWaterUse MeasuringWaterUse = Rio.EFModels.Entities.CustomRichTextTypeMeasuringWaterUse.Instance;
        public static readonly CustomRichTextTypeConfigureWaterTypes ConfigureWaterTypes = Rio.EFModels.Entities.CustomRichTextTypeConfigureWaterTypes.Instance;
        public static readonly CustomRichTextTypeCreateWaterTransactions CreateWaterTransactions = Rio.EFModels.Entities.CustomRichTextTypeCreateWaterTransactions.Instance;
        public static readonly CustomRichTextTypeTrainingVideos TrainingVideos = Rio.EFModels.Entities.CustomRichTextTypeTrainingVideos.Instance;
        public static readonly CustomRichTextTypeCreateUserProfile CreateUserProfile = Rio.EFModels.Entities.CustomRichTextTypeCreateUserProfile.Instance;
        public static readonly CustomRichTextTypeCreateUserProfileStepOne CreateUserProfileStepOne = Rio.EFModels.Entities.CustomRichTextTypeCreateUserProfileStepOne.Instance;
        public static readonly CustomRichTextTypeCreateUserProfileStepTwo CreateUserProfileStepTwo = Rio.EFModels.Entities.CustomRichTextTypeCreateUserProfileStepTwo.Instance;
        public static readonly CustomRichTextTypeCreateUserProfileStepThree CreateUserProfileStepThree = Rio.EFModels.Entities.CustomRichTextTypeCreateUserProfileStepThree.Instance;
        public static readonly CustomRichTextTypeWaterAccountsAdd WaterAccountsAdd = Rio.EFModels.Entities.CustomRichTextTypeWaterAccountsAdd.Instance;
        public static readonly CustomRichTextTypeWaterAccountsAddLegalText WaterAccountsAddLegalText = Rio.EFModels.Entities.CustomRichTextTypeWaterAccountsAddLegalText.Instance;
        public static readonly CustomRichTextTypeWaterAccountsInvite WaterAccountsInvite = Rio.EFModels.Entities.CustomRichTextTypeWaterAccountsInvite.Instance;
        public static readonly CustomRichTextTypeParcelList ParcelList = Rio.EFModels.Entities.CustomRichTextTypeParcelList.Instance;
        public static readonly CustomRichTextTypeOpenETIntegration OpenETIntegration = Rio.EFModels.Entities.CustomRichTextTypeOpenETIntegration.Instance;
        public static readonly CustomRichTextTypeParcelUpdateLayer ParcelUpdateLayer = Rio.EFModels.Entities.CustomRichTextTypeParcelUpdateLayer.Instance;
        public static readonly CustomRichTextTypeInactiveParcelList InactiveParcelList = Rio.EFModels.Entities.CustomRichTextTypeInactiveParcelList.Instance;
        public static readonly CustomRichTextTypeAccountReconciliationReport AccountReconciliationReport = Rio.EFModels.Entities.CustomRichTextTypeAccountReconciliationReport.Instance;
        public static readonly CustomRichTextTypeParcelLedgerCreate ParcelLedgerCreate = Rio.EFModels.Entities.CustomRichTextTypeParcelLedgerCreate.Instance;
        public static readonly CustomRichTextTypeParcelLedgerBulkCreate ParcelLedgerBulkCreate = Rio.EFModels.Entities.CustomRichTextTypeParcelLedgerBulkCreate.Instance;
        public static readonly CustomRichTextTypeParcelLedgerCsvUploadSupply ParcelLedgerCsvUploadSupply = Rio.EFModels.Entities.CustomRichTextTypeParcelLedgerCsvUploadSupply.Instance;
        public static readonly CustomRichTextTypeWebsiteFooter WebsiteFooter = Rio.EFModels.Entities.CustomRichTextTypeWebsiteFooter.Instance;
        public static readonly CustomRichTextTypePurchasedDescription PurchasedDescription = Rio.EFModels.Entities.CustomRichTextTypePurchasedDescription.Instance;
        public static readonly CustomRichTextTypeSoldDescription SoldDescription = Rio.EFModels.Entities.CustomRichTextTypeSoldDescription.Instance;
        public static readonly CustomRichTextTypeTagList TagList = Rio.EFModels.Entities.CustomRichTextTypeTagList.Instance;
        public static readonly CustomRichTextTypeBulkTagParcels BulkTagParcels = Rio.EFModels.Entities.CustomRichTextTypeBulkTagParcels.Instance;
        public static readonly CustomRichTextTypeTransactionHistory TransactionHistory = Rio.EFModels.Entities.CustomRichTextTypeTransactionHistory.Instance;
        public static readonly CustomRichTextTypeParcelLedgerCsvUploadUsage ParcelLedgerCsvUploadUsage = Rio.EFModels.Entities.CustomRichTextTypeParcelLedgerCsvUploadUsage.Instance;

        public static readonly List<CustomRichTextType> All;
        public static readonly List<CustomRichTextTypeDto> AllAsDto;
        public static readonly ReadOnlyDictionary<int, CustomRichTextType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, CustomRichTextTypeDto> AllAsDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static CustomRichTextType()
        {
            All = new List<CustomRichTextType> { HomePage, Contact, FrequentlyAskedQuestions, AboutGET, Disclaimer, PlatformOverview, MeasuringWaterUse, ConfigureWaterTypes, CreateWaterTransactions, TrainingVideos, CreateUserProfile, CreateUserProfileStepOne, CreateUserProfileStepTwo, CreateUserProfileStepThree, WaterAccountsAdd, WaterAccountsAddLegalText, WaterAccountsInvite, ParcelList, OpenETIntegration, ParcelUpdateLayer, InactiveParcelList, AccountReconciliationReport, ParcelLedgerCreate, ParcelLedgerBulkCreate, ParcelLedgerCsvUploadSupply, WebsiteFooter, PurchasedDescription, SoldDescription, TagList, BulkTagParcels, TransactionHistory, ParcelLedgerCsvUploadUsage };
            AllAsDto = new List<CustomRichTextTypeDto> { HomePage.AsDto(), Contact.AsDto(), FrequentlyAskedQuestions.AsDto(), AboutGET.AsDto(), Disclaimer.AsDto(), PlatformOverview.AsDto(), MeasuringWaterUse.AsDto(), ConfigureWaterTypes.AsDto(), CreateWaterTransactions.AsDto(), TrainingVideos.AsDto(), CreateUserProfile.AsDto(), CreateUserProfileStepOne.AsDto(), CreateUserProfileStepTwo.AsDto(), CreateUserProfileStepThree.AsDto(), WaterAccountsAdd.AsDto(), WaterAccountsAddLegalText.AsDto(), WaterAccountsInvite.AsDto(), ParcelList.AsDto(), OpenETIntegration.AsDto(), ParcelUpdateLayer.AsDto(), InactiveParcelList.AsDto(), AccountReconciliationReport.AsDto(), ParcelLedgerCreate.AsDto(), ParcelLedgerBulkCreate.AsDto(), ParcelLedgerCsvUploadSupply.AsDto(), WebsiteFooter.AsDto(), PurchasedDescription.AsDto(), SoldDescription.AsDto(), TagList.AsDto(), BulkTagParcels.AsDto(), TransactionHistory.AsDto(), ParcelLedgerCsvUploadUsage.AsDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, CustomRichTextType>(All.ToDictionary(x => x.CustomRichTextTypeID));
            AllAsDtoLookupDictionary = new ReadOnlyDictionary<int, CustomRichTextTypeDto>(AllAsDto.ToDictionary(x => x.CustomRichTextTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected CustomRichTextType(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName)
        {
            CustomRichTextTypeID = customRichTextTypeID;
            CustomRichTextTypeName = customRichTextTypeName;
            CustomRichTextTypeDisplayName = customRichTextTypeDisplayName;
        }

        [Key]
        public int CustomRichTextTypeID { get; private set; }
        public string CustomRichTextTypeName { get; private set; }
        public string CustomRichTextTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return CustomRichTextTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(CustomRichTextType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.CustomRichTextTypeID == CustomRichTextTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as CustomRichTextType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return CustomRichTextTypeID;
        }

        public static bool operator ==(CustomRichTextType left, CustomRichTextType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomRichTextType left, CustomRichTextType right)
        {
            return !Equals(left, right);
        }

        public CustomRichTextTypeEnum ToEnum => (CustomRichTextTypeEnum)GetHashCode();

        public static CustomRichTextType ToType(int enumValue)
        {
            return ToType((CustomRichTextTypeEnum)enumValue);
        }

        public static CustomRichTextType ToType(CustomRichTextTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case CustomRichTextTypeEnum.AboutGET:
                    return AboutGET;
                case CustomRichTextTypeEnum.AccountReconciliationReport:
                    return AccountReconciliationReport;
                case CustomRichTextTypeEnum.BulkTagParcels:
                    return BulkTagParcels;
                case CustomRichTextTypeEnum.ConfigureWaterTypes:
                    return ConfigureWaterTypes;
                case CustomRichTextTypeEnum.Contact:
                    return Contact;
                case CustomRichTextTypeEnum.CreateUserProfile:
                    return CreateUserProfile;
                case CustomRichTextTypeEnum.CreateUserProfileStepOne:
                    return CreateUserProfileStepOne;
                case CustomRichTextTypeEnum.CreateUserProfileStepThree:
                    return CreateUserProfileStepThree;
                case CustomRichTextTypeEnum.CreateUserProfileStepTwo:
                    return CreateUserProfileStepTwo;
                case CustomRichTextTypeEnum.CreateWaterTransactions:
                    return CreateWaterTransactions;
                case CustomRichTextTypeEnum.Disclaimer:
                    return Disclaimer;
                case CustomRichTextTypeEnum.FrequentlyAskedQuestions:
                    return FrequentlyAskedQuestions;
                case CustomRichTextTypeEnum.HomePage:
                    return HomePage;
                case CustomRichTextTypeEnum.InactiveParcelList:
                    return InactiveParcelList;
                case CustomRichTextTypeEnum.MeasuringWaterUse:
                    return MeasuringWaterUse;
                case CustomRichTextTypeEnum.OpenETIntegration:
                    return OpenETIntegration;
                case CustomRichTextTypeEnum.ParcelLedgerBulkCreate:
                    return ParcelLedgerBulkCreate;
                case CustomRichTextTypeEnum.ParcelLedgerCreate:
                    return ParcelLedgerCreate;
                case CustomRichTextTypeEnum.ParcelLedgerCsvUploadSupply:
                    return ParcelLedgerCsvUploadSupply;
                case CustomRichTextTypeEnum.ParcelLedgerCsvUploadUsage:
                    return ParcelLedgerCsvUploadUsage;
                case CustomRichTextTypeEnum.ParcelList:
                    return ParcelList;
                case CustomRichTextTypeEnum.ParcelUpdateLayer:
                    return ParcelUpdateLayer;
                case CustomRichTextTypeEnum.PlatformOverview:
                    return PlatformOverview;
                case CustomRichTextTypeEnum.PurchasedDescription:
                    return PurchasedDescription;
                case CustomRichTextTypeEnum.SoldDescription:
                    return SoldDescription;
                case CustomRichTextTypeEnum.TagList:
                    return TagList;
                case CustomRichTextTypeEnum.TrainingVideos:
                    return TrainingVideos;
                case CustomRichTextTypeEnum.TransactionHistory:
                    return TransactionHistory;
                case CustomRichTextTypeEnum.WaterAccountsAdd:
                    return WaterAccountsAdd;
                case CustomRichTextTypeEnum.WaterAccountsAddLegalText:
                    return WaterAccountsAddLegalText;
                case CustomRichTextTypeEnum.WaterAccountsInvite:
                    return WaterAccountsInvite;
                case CustomRichTextTypeEnum.WebsiteFooter:
                    return WebsiteFooter;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum CustomRichTextTypeEnum
    {
        HomePage = 1,
        Contact = 2,
        FrequentlyAskedQuestions = 3,
        AboutGET = 4,
        Disclaimer = 5,
        PlatformOverview = 6,
        MeasuringWaterUse = 7,
        ConfigureWaterTypes = 8,
        CreateWaterTransactions = 9,
        TrainingVideos = 10,
        CreateUserProfile = 11,
        CreateUserProfileStepOne = 12,
        CreateUserProfileStepTwo = 13,
        CreateUserProfileStepThree = 14,
        WaterAccountsAdd = 15,
        WaterAccountsAddLegalText = 16,
        WaterAccountsInvite = 17,
        ParcelList = 18,
        OpenETIntegration = 19,
        ParcelUpdateLayer = 20,
        InactiveParcelList = 21,
        AccountReconciliationReport = 22,
        ParcelLedgerCreate = 23,
        ParcelLedgerBulkCreate = 24,
        ParcelLedgerCsvUploadSupply = 25,
        WebsiteFooter = 26,
        PurchasedDescription = 28,
        SoldDescription = 29,
        TagList = 30,
        BulkTagParcels = 31,
        TransactionHistory = 32,
        ParcelLedgerCsvUploadUsage = 33
    }

    public partial class CustomRichTextTypeHomePage : CustomRichTextType
    {
        private CustomRichTextTypeHomePage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeHomePage Instance = new CustomRichTextTypeHomePage(1, @"HomePage", @"Home Page");
    }

    public partial class CustomRichTextTypeContact : CustomRichTextType
    {
        private CustomRichTextTypeContact(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeContact Instance = new CustomRichTextTypeContact(2, @"Contact", @"Contact");
    }

    public partial class CustomRichTextTypeFrequentlyAskedQuestions : CustomRichTextType
    {
        private CustomRichTextTypeFrequentlyAskedQuestions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeFrequentlyAskedQuestions Instance = new CustomRichTextTypeFrequentlyAskedQuestions(3, @"FrequentlyAskedQuestions", @"Frequently Asked Questons");
    }

    public partial class CustomRichTextTypeAboutGET : CustomRichTextType
    {
        private CustomRichTextTypeAboutGET(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeAboutGET Instance = new CustomRichTextTypeAboutGET(4, @"AboutGET", @"About GET");
    }

    public partial class CustomRichTextTypeDisclaimer : CustomRichTextType
    {
        private CustomRichTextTypeDisclaimer(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeDisclaimer Instance = new CustomRichTextTypeDisclaimer(5, @"Disclaimer", @"Disclaimer");
    }

    public partial class CustomRichTextTypePlatformOverview : CustomRichTextType
    {
        private CustomRichTextTypePlatformOverview(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypePlatformOverview Instance = new CustomRichTextTypePlatformOverview(6, @"PlatformOverview", @"Platform Overview");
    }

    public partial class CustomRichTextTypeMeasuringWaterUse : CustomRichTextType
    {
        private CustomRichTextTypeMeasuringWaterUse(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeMeasuringWaterUse Instance = new CustomRichTextTypeMeasuringWaterUse(7, @"MeasuringWaterUse", @"Measuring Water Use With OpenET");
    }

    public partial class CustomRichTextTypeConfigureWaterTypes : CustomRichTextType
    {
        private CustomRichTextTypeConfigureWaterTypes(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeConfigureWaterTypes Instance = new CustomRichTextTypeConfigureWaterTypes(8, @"ConfigureWaterTypes", @"Configure Water Types");
    }

    public partial class CustomRichTextTypeCreateWaterTransactions : CustomRichTextType
    {
        private CustomRichTextTypeCreateWaterTransactions(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeCreateWaterTransactions Instance = new CustomRichTextTypeCreateWaterTransactions(9, @"CreateWaterTransactions", @"Create Water Transactions");
    }

    public partial class CustomRichTextTypeTrainingVideos : CustomRichTextType
    {
        private CustomRichTextTypeTrainingVideos(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeTrainingVideos Instance = new CustomRichTextTypeTrainingVideos(10, @"TrainingVideos", @"Training Videos");
    }

    public partial class CustomRichTextTypeCreateUserProfile : CustomRichTextType
    {
        private CustomRichTextTypeCreateUserProfile(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeCreateUserProfile Instance = new CustomRichTextTypeCreateUserProfile(11, @"CreateUserProfile", @"Create User Profile");
    }

    public partial class CustomRichTextTypeCreateUserProfileStepOne : CustomRichTextType
    {
        private CustomRichTextTypeCreateUserProfileStepOne(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeCreateUserProfileStepOne Instance = new CustomRichTextTypeCreateUserProfileStepOne(12, @"CreateUserProfileStepOne", @"Create User Profile Step One");
    }

    public partial class CustomRichTextTypeCreateUserProfileStepTwo : CustomRichTextType
    {
        private CustomRichTextTypeCreateUserProfileStepTwo(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeCreateUserProfileStepTwo Instance = new CustomRichTextTypeCreateUserProfileStepTwo(13, @"CreateUserProfileStepTwo", @"Create User Profile Step Two");
    }

    public partial class CustomRichTextTypeCreateUserProfileStepThree : CustomRichTextType
    {
        private CustomRichTextTypeCreateUserProfileStepThree(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeCreateUserProfileStepThree Instance = new CustomRichTextTypeCreateUserProfileStepThree(14, @"CreateUserProfileStepThree", @"Create User Profile Step Three");
    }

    public partial class CustomRichTextTypeWaterAccountsAdd : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountsAdd(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeWaterAccountsAdd Instance = new CustomRichTextTypeWaterAccountsAdd(15, @"WaterAccountsAdd", @"Water Accounts Add");
    }

    public partial class CustomRichTextTypeWaterAccountsAddLegalText : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountsAddLegalText(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeWaterAccountsAddLegalText Instance = new CustomRichTextTypeWaterAccountsAddLegalText(16, @"WaterAccountsAddLegalText", @"Water Accounts Add Legal Text");
    }

    public partial class CustomRichTextTypeWaterAccountsInvite : CustomRichTextType
    {
        private CustomRichTextTypeWaterAccountsInvite(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeWaterAccountsInvite Instance = new CustomRichTextTypeWaterAccountsInvite(17, @"WaterAccountsInvite", @"Water Accounts Invite");
    }

    public partial class CustomRichTextTypeParcelList : CustomRichTextType
    {
        private CustomRichTextTypeParcelList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelList Instance = new CustomRichTextTypeParcelList(18, @"ParcelList", @"Parcel List");
    }

    public partial class CustomRichTextTypeOpenETIntegration : CustomRichTextType
    {
        private CustomRichTextTypeOpenETIntegration(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeOpenETIntegration Instance = new CustomRichTextTypeOpenETIntegration(19, @"OpenETIntegration", @"OpenET Integration");
    }

    public partial class CustomRichTextTypeParcelUpdateLayer : CustomRichTextType
    {
        private CustomRichTextTypeParcelUpdateLayer(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelUpdateLayer Instance = new CustomRichTextTypeParcelUpdateLayer(20, @"ParcelUpdateLayer", @"Parcel Update Layer");
    }

    public partial class CustomRichTextTypeInactiveParcelList : CustomRichTextType
    {
        private CustomRichTextTypeInactiveParcelList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeInactiveParcelList Instance = new CustomRichTextTypeInactiveParcelList(21, @"InactiveParcelList", @"Inactive Parcel List");
    }

    public partial class CustomRichTextTypeAccountReconciliationReport : CustomRichTextType
    {
        private CustomRichTextTypeAccountReconciliationReport(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeAccountReconciliationReport Instance = new CustomRichTextTypeAccountReconciliationReport(22, @"AccountReconciliationReport", @"Account Reconciliation Report");
    }

    public partial class CustomRichTextTypeParcelLedgerCreate : CustomRichTextType
    {
        private CustomRichTextTypeParcelLedgerCreate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelLedgerCreate Instance = new CustomRichTextTypeParcelLedgerCreate(23, @"ParcelLedgerCreate", @"Create New Transaction");
    }

    public partial class CustomRichTextTypeParcelLedgerBulkCreate : CustomRichTextType
    {
        private CustomRichTextTypeParcelLedgerBulkCreate(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelLedgerBulkCreate Instance = new CustomRichTextTypeParcelLedgerBulkCreate(24, @"ParcelLedgerBulkCreate", @"Create Bulk Transaction");
    }

    public partial class CustomRichTextTypeParcelLedgerCsvUploadSupply : CustomRichTextType
    {
        private CustomRichTextTypeParcelLedgerCsvUploadSupply(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelLedgerCsvUploadSupply Instance = new CustomRichTextTypeParcelLedgerCsvUploadSupply(25, @"ParcelLedgerCsvUploadSupply", @"Parcel Ledger CSV Upload (Supply)");
    }

    public partial class CustomRichTextTypeWebsiteFooter : CustomRichTextType
    {
        private CustomRichTextTypeWebsiteFooter(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeWebsiteFooter Instance = new CustomRichTextTypeWebsiteFooter(26, @"WebsiteFooter", @"Website Footer");
    }

    public partial class CustomRichTextTypePurchasedDescription : CustomRichTextType
    {
        private CustomRichTextTypePurchasedDescription(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypePurchasedDescription Instance = new CustomRichTextTypePurchasedDescription(28, @"PurchasedDescription", @"Purchased Water Description");
    }

    public partial class CustomRichTextTypeSoldDescription : CustomRichTextType
    {
        private CustomRichTextTypeSoldDescription(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeSoldDescription Instance = new CustomRichTextTypeSoldDescription(29, @"SoldDescription", @"Sold Water Description");
    }

    public partial class CustomRichTextTypeTagList : CustomRichTextType
    {
        private CustomRichTextTypeTagList(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeTagList Instance = new CustomRichTextTypeTagList(30, @"TagList", @"Tag List");
    }

    public partial class CustomRichTextTypeBulkTagParcels : CustomRichTextType
    {
        private CustomRichTextTypeBulkTagParcels(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeBulkTagParcels Instance = new CustomRichTextTypeBulkTagParcels(31, @"BulkTagParcels", @"Bulk Tag Parcels");
    }

    public partial class CustomRichTextTypeTransactionHistory : CustomRichTextType
    {
        private CustomRichTextTypeTransactionHistory(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeTransactionHistory Instance = new CustomRichTextTypeTransactionHistory(32, @"TransactionHistory", @"Transaction History");
    }

    public partial class CustomRichTextTypeParcelLedgerCsvUploadUsage : CustomRichTextType
    {
        private CustomRichTextTypeParcelLedgerCsvUploadUsage(int customRichTextTypeID, string customRichTextTypeName, string customRichTextTypeDisplayName) : base(customRichTextTypeID, customRichTextTypeName, customRichTextTypeDisplayName) {}
        public static readonly CustomRichTextTypeParcelLedgerCsvUploadUsage Instance = new CustomRichTextTypeParcelLedgerCsvUploadUsage(33, @"ParcelLedgerCsvUploadUsage", @"Parcel Ledger CSV Upload (Usage)");
    }
}