using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.PositionService.Mappers.Models.Interfaces
{
    [AutoInject]
    public interface IPositionInfoMapper
    {
        PositionInfo Map(DbPosition position);
    }
}
