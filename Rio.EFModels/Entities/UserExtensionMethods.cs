using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class UserExtensionMethods
    {
        static partial void DoCustomMappings(User user, UserDto userDto)
        {
            userDto.FullName = $"{user.FirstName} {user.LastName}";
        }

        static partial void DoCustomSimpleDtoMappings(User user, UserSimpleDto userSimpleDto)
        {
            userSimpleDto.FullName = $"{user.FirstName} {user.LastName}";
        }
    }
}