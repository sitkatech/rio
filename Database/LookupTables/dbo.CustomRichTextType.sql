MERGE INTO dbo.CustomRichTextType AS Target
USING (VALUES
(1, 'HomePage', 'Home Page'),
(2, 'Contact', 'Contact'),
(3, 'FrequentlyAskedQuestions', 'Frequently Asked Questons'),
(4, 'AboutGET', 'About GET'),
(5, 'Disclaimer', 'Disclaimer'),
(6, 'PlatformOverview', 'Platform Overview'),
(7, 'MeasuringWaterUse', 'Measuring Water Use With OpenET'),
(8, 'ConfigureWaterTypes', 'Configure Water Types'),
(9, 'SetWaterAllocation', 'SetWaterAllocation'),
(10, 'TrainingVideos', 'Training Videos'),
(11, 'CreateUserProfile', 'Create User Profile'),
(12, 'CreateUserProfileStepOne', 'Create User Profile Step One'),
(13, 'CreateUserProfileStepTwo', 'Create User Profile Step Two'),
(14, 'CreateUserProfileStepThree', 'Create User Profile Step Three'),
(15, 'WaterAccountsAdd', 'Water Accounts Add'),
(16, 'WaterAccountsAddLegalText', 'Water Accounts Add Legal Text'),
(17, 'WaterAccountsInvite', 'Water Accounts Invite')
)
AS Source (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
ON Target.CustomRichTextTypeID = Source.CustomRichTextTypeID
WHEN MATCHED THEN
UPDATE SET
	CustomRichTextTypeName = Source.CustomRichTextTypeName,
	CustomRichTextTypeDisplayName = Source.CustomRichTextTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
	VALUES (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
