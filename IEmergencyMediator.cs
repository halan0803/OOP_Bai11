/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Hợp đồng điều phối: observer không cần biết các thiết bị cụ thể
    public interface IEmergencyMediator
    {
        void HandleEmergency(AlertEvent alertEvent);
    }
}