using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Bao.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BaoUser class
    public class BaoUser : IdentityUser
    {
        [PersonalData]
        [StringLength(50, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [PersonalData]
        [StringLength(50, MinimumLength = 2)]
        public string? LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return LastName + " " + FirstName; } }

        [PersonalData]
        [StringLength(10)]
        public string? Gender { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Address fields
        [PersonalData]
        [StringLength(100)]
        public string? AddressLine1 { get; set; }

        [PersonalData]
        [StringLength(100)]
        public string? AddressLine2 { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? City { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? State { get; set; }

        [PersonalData]
        [StringLength(20)]
        public string? PostalCode { get; set; }

        [PersonalData]
        [StringLength(50)]
        public string? Country { get; set; }

        [PersonalData]
        [StringLength(20)]
        public string? Role { get; set; }

        [PersonalData]
        public DateTime CreateAt { get; set; }

        [PersonalData]
        public DateTime ModifiedAt { get; set; }
    }
}
