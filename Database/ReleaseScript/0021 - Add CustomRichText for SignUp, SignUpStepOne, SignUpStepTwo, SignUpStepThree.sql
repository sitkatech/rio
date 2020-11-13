insert into dbo.CustomRichTextType (CustomRichTextTypeID, CustomRichTextTypeName, CustomRichTextTypeDisplayName)
values (11, 'CreateUserProfile', 'Create User Profile'),
(12, 'CreateUserProfileStepOne', 'Create User Profile Step One'),
(13, 'CreateUserProfileStepTwo', 'Create User Profile Step Two'),
(14, 'CreateUserProfileStepThree', 'Create User Profile Step Three'),
(15, 'WaterAccountsAdd', 'Water Accounts Add'),
(16, 'WaterAccountsAddLegalText', 'Water Accounts Add Legal Text')

insert into dbo.CustomRichText (CustomRichTextTypeID, CustomRichTextContent)
values (11, '<p>Welcome to the Rosedale-Rio Bravo Water Storage District Water Accounting Platform. ... More Instructions</p>'),
(12, '<h4>Step 1: Create your User Profile</h4><p>To access the Water Accounting Platform first you must set up a username and password. Account creation, password resets, forgotten username assistance, are managed by users with the Keystone Identity Management.</p>'),
(13, '<h4>Step 2: Add your Water Accounts to your Profile</h4><p>After you create a Keystone Profile and Log In to the platform, one more step is required to access your Landowner Dashboard. Enter the unique Account Verification Key you have received from the district to finish configuring your account.</p>'),
(14, '<h4>Step 3 (Optional): Invite business partners or family members and manage access to your accounts</h4><p>Additional text to ensure proper spacing.</p>'),
(15, '<p>Enter the Account Verification Key you received in the mailer. After you press "Find" the system will provide details of the account and configure your profile. If you have more than one code, enter them one at a time and press Find Account.</p>'),
(16, '<p>Only landowners or their authorized representatives may use this form to gain access to a Water Account. If you are not the landowner or an authorized representative please contact the landowner. Please type your digital signature to authorize that you are the landowner or a legally authorized representative.</p>')