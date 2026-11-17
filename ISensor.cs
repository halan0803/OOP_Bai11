/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Giao diện chung để module giám sát đọc giá trị cảm biến
    public interface ISensor
    {
        double ReadValue();
        string GetUnit();
    }
}