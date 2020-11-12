insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (11, 'SignUp', 'Sign Up'),
(12, 'SignUpStepOne', 'Sign Up Step One'),
(13, 'SignUpStepTwo', 'Sign Up Step Two'),
(14, 'SignUpStepThree', 'Sign Up Step Three'),
(15, 'WaterAccountsAdd', 'Water Accounts Add'),
(16, 'WaterAccountsAddLegalText', 'Water Accounts Add Legal Text')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (11, 'Default Sign Up Content'),
(12, 'Default Sign Up Step One Content'),
(13, 'Default Sign Up Step Two Content'),
(14, 'Default Sign Up Step Three Content'),
(15, 'Default Add Water Accounts Content'),
(16, 'Default Add Water Accounts Legal Text')