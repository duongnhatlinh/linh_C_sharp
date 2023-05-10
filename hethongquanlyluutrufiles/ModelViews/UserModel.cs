using System.ComponentModel.DataAnnotations;
using System;
using hethongquanlyluutrufiles.Models;

namespace hethongquanlyluutrufiles.ModelViews
{
    public class UserModel
    {
        [Required(ErrorMessage = ("Vui lòng nhập ngày sinh"))]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [Required(ErrorMessage = ("Vui lựa chọn giới tính"))]
        public string Sex { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = ("Vui lòng nhập địa chỉ"))]
        public string Address { get; set; }
        [MaxLength(15)]
        [Required(ErrorMessage = ("Vui lòng nhập CMND"))]
        public string Cmnd { get; set; }
        [Required(ErrorMessage = ("Vui lòng chọn vai trò"))]
        public int RoleId { get; set; }
        [Required(ErrorMessage = ("Vui lòng chọn phòng ban"))]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = ("Vui lòng chọn kiểu công việc"))]
        public string Typework { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = ("Vui lòng nhập công ty"))]
        public string Company { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = ("Vui lòng nhập nơi làm việc"))]
        public string Workplace { get; set; }

        public virtual Department Department { get; set; }
        public virtual Role Role { get; set; }
    }
}
