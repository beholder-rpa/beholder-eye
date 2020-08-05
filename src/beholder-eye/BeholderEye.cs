namespace beholder_eye
{
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class BeholderEye
    {
        private const string AlignmentMapKey = "Beholder_Eye_Alignment.json";

        private readonly object _snapshotLock = new object();
        private readonly object _alignLock = new object();
        private readonly ILogger _logger;
        private readonly HashAlgorithm _hashAlgorithm;

        public BeholderEye(ILogger<BeholderEye> logger, HashAlgorithm hashAlgorithm)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm));
        }

        public HubConnection NexusConnection
        {
            get;
            set;
        }

        public ConnectionMultiplexer Redis
        {
            get;
            set;
        }

        public SnapshotRequest SnapshotRequest
        {
            get;
            set;
        }
        public AlignRequest AlignRequest
        {
            get;
            set;
        }

        public Task ObserveWithUnwaveringSight(ObservationRequest observerInfo, CancellationToken token)
        {
            if (observerInfo == null)
            {
                throw new ArgumentNullException(nameof(observerInfo));
            }

            var streamDesktopThumbnails = true;
            if (observerInfo.StreamDesktopThumbnail.HasValue)
            {
                streamDesktopThumbnails = observerInfo.StreamDesktopThumbnail.Value;
            }

            var desktopThumbnailStreamSettings = new DesktopThumbnailStreamSettings();
            if (observerInfo.DesktopThumbnailStreamSettings != null)
            {
                if (observerInfo.DesktopThumbnailStreamSettings.MaxFps.HasValue && observerInfo.DesktopThumbnailStreamSettings.MaxFps.Value > 0)
                {
                    desktopThumbnailStreamSettings.MaxFps = observerInfo.DesktopThumbnailStreamSettings.MaxFps.Value;
                }

                if (observerInfo.DesktopThumbnailStreamSettings.ScaleFactor.HasValue && observerInfo.DesktopThumbnailStreamSettings.ScaleFactor.Value > 0)
                {
                    desktopThumbnailStreamSettings.ScaleFactor = observerInfo.DesktopThumbnailStreamSettings.ScaleFactor.Value;
                }
            }

            var streamPointerPosition = true;
            if (observerInfo.StreamPointerPosition.HasValue)
            {
                streamPointerPosition = observerInfo.StreamPointerPosition.Value;
            }

            // Observe the screen on a seperate thread.
            return Task.Factory.StartNew(() =>
            {
                var duplicatorInstance = new DesktopDuplicator(_logger, observerInfo.AdapterIndex ?? 0, observerInfo.DeviceIndex ?? 0);

                int? lastMatrixFrameIdSent = null;
                DateTime? lastdesktopThumbnailSent = null;
                PointerPosition lastPointerPosition = null;
                int lastWidth = 0, lastHeight = 0;

                _logger.LogInformation("The cold stare of the Beholder's unwavering glare is now focused upon the screen...");

                //TODO: Process each frame on a separate thread -- we get a "Object is in use elsewhere" from the DesktopImage when we do this though.
                foreach (var desktopFrame in duplicatorInstance.DuplicateDesktop(token))
                {
                    if (token.IsCancellationRequested || desktopFrame == null || desktopFrame.DesktopWidth == 0 || desktopFrame.DesktopHeight == 0 || desktopFrame.IsDesktopImageBufferEmpty)
                    {
                        continue;
                    }

                    if (lastWidth != desktopFrame.DesktopWidth || lastHeight != desktopFrame.DesktopHeight)
                    {
                        NexusConnection?.SendAsync("EyeReport", "ScreenDimensions", new object[] { desktopFrame.DesktopWidth, desktopFrame.DesktopHeight });
                        lastWidth = desktopFrame.DesktopWidth;
                        lastHeight = desktopFrame.DesktopHeight;
                    }

                    if (streamDesktopThumbnails)
                    {
                        if (!lastdesktopThumbnailSent.HasValue || DateTime.Now.Subtract(lastdesktopThumbnailSent.Value) > TimeSpan.FromSeconds(desktopThumbnailStreamSettings.MaxFps.Value))
                        {
                            var width = (int)Math.Ceiling(desktopFrame.DesktopWidth * desktopThumbnailStreamSettings.ScaleFactor.Value);
                            var height = (int)Math.Ceiling(desktopFrame.DesktopHeight * desktopThumbnailStreamSettings.ScaleFactor.Value);

                            var thumbnailImage = desktopFrame.GetThumbnailImage(width, height);

                            var now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
                            var key = $"Eye_Thumb_{now}.png";

                            try
                            {
                                var db = Redis.GetDatabase();
                                db.StringSet(key, thumbnailImage, TimeSpan.FromSeconds(30));
                                NexusConnection?.SendAsync("EyeReport", "Thumbnail", new object[] { key, width, height });
                            }
                            catch (RedisException ex)
                            {
                                _logger.LogError(ex, $"Unable to store thumbnail in redis. {ex.Message}");
                            }
                        }
                    }

                    if (streamPointerPosition)
                    {
                        var newPointerPosition = desktopFrame.PointerPosition;

                        if (lastPointerPosition == null || (lastPointerPosition.X != newPointerPosition.X || lastPointerPosition.Y != newPointerPosition.Y || lastPointerPosition.Visible != newPointerPosition.Visible))
                        {
                            NexusConnection?.SendAsync("EyeReport", "PointerPosition", new object[] { newPointerPosition });
                            lastPointerPosition = newPointerPosition;
                        }

                        var pointerData = desktopFrame.GetPointerImage();
                        if (pointerData != null && lastPointerPosition.Visible.Value)
                        {
                            var hash = _hashAlgorithm.ComputeHash(pointerData);
                            var key = $"Eye_Pointer_{Convert.ToBase64String(hash)}.png";

                            try
                            {
                                var db = Redis.GetDatabase();
                                if (!db.KeyExists(key))
                                {
                                    db.StringSet(key, pointerData);
                                }
                            }
                            catch (RedisException ex)
                            {
                                _logger.LogError(ex, $"Unable to store pointer in redis. {ex.Message}");
                            }
                            finally
                            {
                                NexusConnection?.SendAsync("EyeReport", "PointerShape", new object[] { desktopFrame.PointerShape, key });
                            }
                        }
                    }

                    // Double-check locking for a snapshot request.
                    if (SnapshotRequest != null)
                    {
                        lock (_snapshotLock)
                        {
                            if (SnapshotRequest != null)
                            {
                                if (SnapshotRequest.ScaleFactor.HasValue == false)
                                {
                                    SnapshotRequest.ScaleFactor = 1.0;
                                }

                                if (SnapshotRequest.Format.HasValue == false)
                                {
                                    SnapshotRequest.Format = SnapshotFormat.Png;
                                }

                                var width = (int)Math.Ceiling(desktopFrame.DesktopWidth * SnapshotRequest.ScaleFactor.Value);
                                var height = (int)Math.Ceiling(desktopFrame.DesktopHeight * SnapshotRequest.ScaleFactor.Value);

                                var snapshot = desktopFrame.GetSnapshot(width, height, SnapshotRequest.Format.Value);

                                var now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
                                var key = $"Eye_Snapshot_{now}";
                                switch (SnapshotRequest.Format)
                                {
                                    case SnapshotFormat.Jpeg:
                                        key += ".jpg";
                                        break;
                                    case SnapshotFormat.Png:
                                    default:
                                        key += ".png";
                                        break;
                                }

                                try
                                {

                                    var db = Redis.GetDatabase();
                                    db.StringSet(key, snapshot, TimeSpan.FromHours(2));

                                    if (SnapshotRequest.Metadata != null)
                                    {
                                        var metadataKey = $"Eye_Snapshot_Metadata_{now}";
                                        db.StringSet(metadataKey, JsonSerializer.Serialize(SnapshotRequest.Metadata), TimeSpan.FromHours(2));
                                    }

                                    NexusConnection?.SendAsync("EyeReport", "Snapshot", new object[] { key, width, height });
                                }
                                catch (RedisException ex)
                                {
                                    _logger.LogError(ex, $"Unable to store snapshot in redis. {ex.Message}");
                                }

                                // We've taken a snapshot, clear the request.
                                SnapshotRequest = null;
                            }
                        }
                    }

                    if (AlignRequest != null)
                    {
                        lock (_alignLock)
                        {
                            if (AlignRequest != null)
                            {
                                var pixelSize = 2;
                                if (AlignRequest.PixelSIze.HasValue && AlignRequest.PixelSIze.Value > 0)
                                {
                                    pixelSize = AlignRequest.PixelSIze.Value;
                                }

                                var map = desktopFrame.GenerateAlignmentMap(pixelSize);

                                try
                                {
                                    var db = Redis.GetDatabase();
                                    db.StringSet(AlignmentMapKey, JsonSerializer.Serialize(map));

                                    NexusConnection?.SendAsync("EyeReport", "AlignmentMapUpdated", new object[] { AlignmentMapKey });
                                }
                                catch (RedisException ex)
                                {
                                    _logger.LogError(ex, $"Unable to store alignment maP in redis. {ex.Message}");
                                }


                                AlignRequest = null;
                            }
                        }
                    }

                    foreach (var region in observerInfo.Regions)
                    {
                        switch (region.Kind)
                        {
                            case ObservationRegionKind.MatrixFrame:
                                if (region.MatrixSettings == null)
                                {
                                    //TODO: some sort of logging that won't generate a bunch of redundant data?
                                    continue;
                                }

                                if (region.MatrixSettings.Map == null)
                                {
                                    try
                                    {
                                        var db = Redis.GetDatabase();
                                        var json = db.StringGet(AlignmentMapKey);
                                        if (json.HasValue)
                                        {
                                            region.MatrixSettings.Map = JsonSerializer.Deserialize<IList<int>>(json);
                                        }

                                    }
                                    catch (RedisException)
                                    {
                                        // Do Nothing
                                    }
                                }

                                if (region.MatrixSettings.DataFormat == null)
                                {
                                    region.MatrixSettings.DataFormat = DataMatrixFormat.MatrixEvents;
                                }

                                var matrixFrame = desktopFrame.DecodeMatrixFrame(region.MatrixSettings);
                                if (matrixFrame != null && lastMatrixFrameIdSent != matrixFrame.FrameId)
                                {
                                    NexusConnection?.SendAsync("EyeReport", "MatrixFrame", new object[] { matrixFrame });
                                    lastMatrixFrameIdSent = matrixFrame.FrameId;
                                }

                                break;
                            case ObservationRegionKind.Image:
                            default:
                                break;
                        }
                    }
                };

                _logger.LogInformation("The Beholder has focused its attention elsewhere.");
            }, token, TaskCreationOptions.None, TaskScheduler.Default);
        }
    }
}
