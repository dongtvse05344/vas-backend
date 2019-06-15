
using Mapster;
using System;
using VAS.Model;
using VAS.ViewModels;

namespace VAS.Utils
{
    public class MapsterConfig
    {
        public MapsterConfig()
        {
        }
        public void Run()
        {
            TypeAdapterConfig<DoctorBasicUpdateModel, DoctorBasic>.NewConfig()
                .Map(des => des.DoctorPro.Language, src => src.Language)
                .Map(des => des.DoctorPro.Degree, src => src.Degree)
                .Map(des => des.DoctorPro.Certification, src => src.Certification)
                .Map(des => des.DoctorPro.Experience, src => src.Experience);

            TypeAdapterConfig<Room, RoomViewModel>.NewConfig()
                .Map(des => des.SpecialityName, src => src.Speciality.Name);

            TypeAdapterConfig<DoctorBasic, DoctorBasicViewModel>.NewConfig()
                .Map(des => des.FullName, src => src.MyUser.FullName)
                .Map(des => des.IsActive, src => src.MyUser.IsActive);

			TypeAdapterConfig<DoctorBasic, DoctorDetailVM>.NewConfig()
				.Map(des => des.IsActive, src => src.MyUser.IsActive);

			TypeAdapterConfig<Nurse, NurseViewModel>.NewConfig()
                .Map(des => des.IsActive, src => src.MyUser.IsActive);

			TypeAdapterConfig<Nurse, NurseDetailVM>.NewConfig()
                .Map(des => des.IsActive, src => src.MyUser.IsActive);

            TypeAdapterConfig<Scheduling, SchedulingViewModel>.NewConfig()
                .Map(des => des.DoctorName, src => src.Doctor.FullName)
                .Map(des => des.NurseName, src => src.Nurse ==null ? "": src.Nurse.FullName)
                .Map(des => des.RoomNumber, src => src.Room.Name +" - Số phòng : " +src.Room.Number);

            TypeAdapterConfig<Scheduling, SchedulingVM2>.NewConfig()
                .Map(des => des.DoctorName, src => src.Doctor.FullName)
                .Map(des => des.NurseName, src => src.Nurse == null ? "" : src.Nurse.FullName)
                .Map(des => des.RoomName, src => src.Room.Name);

            TypeAdapterConfig<Block, BlockBookingViewModel>.NewConfig()
                .Map(des => des.Time, src => src.StartTime.ToString(@"hh\:mm"));

		}
    }
}
