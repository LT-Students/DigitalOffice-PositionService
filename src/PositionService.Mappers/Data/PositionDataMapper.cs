using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Data
{
  public class PositionDataMapper : IPositionDataMapper
  {
    private readonly IPositionUserDataMapper _userMapper;

    public PositionDataMapper(IPositionUserDataMapper userMapper)
    {
      _userMapper = userMapper;
    }

    public PositionData Map(DbPosition position, List<DbUserRate> rate)
    {
      if (position == null)
      {
        return null;
      }

      return new PositionData(
        position.Id,
        position.Name,
        position.Users.Select(u => _userMapper.Map(u, rate.FirstOrDefault(r => r.UserId == u.UserId))).ToList());
    }
  }
}
