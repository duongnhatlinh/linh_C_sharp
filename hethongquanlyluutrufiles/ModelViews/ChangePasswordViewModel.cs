using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace hethongquanlyluutrufiles.ModelViews
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string PasswordCurrent { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string PasswordNew { get; set; }

        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("PasswordNew", ErrorMessage = "Nhập lại mật khẩu không đúng")]
        public string ConfirmPasswordNew { get; set; }
    }
}
