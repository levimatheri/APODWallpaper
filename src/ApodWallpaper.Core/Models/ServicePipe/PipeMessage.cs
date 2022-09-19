using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApodWallpaper.Core.Models.ServicePipe;

[Serializable]
public record PipeMessage(Guid Id, ActionType Action = ActionType.Unknown, string? Text = null);
