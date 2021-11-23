using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using YoutubeExplode;

namespace FindTheThing.DiscordBot.Commands;

public class UserCommands : CommandGroup
{
    private readonly FeedbackService _feedbackService;
    private readonly ICommandContext _commandContext;
    private readonly YoutubeClient _youtubeClient;
    private const int MAX_VIDEOS = 10;

    public UserCommands(FeedbackService feedbackService, ICommandContext commandContext, YoutubeClient youtubeClient)
    {
        _feedbackService = feedbackService;
        _commandContext = commandContext;
        _youtubeClient = youtubeClient;
    }

    [Command("youtube")]
    [CommandType(ApplicationCommandType.ChatInput)]
    [Description("Search YouTube for a specific video")]
    public async Task<IResult> YouTubeAsync([Description("Search query")] string searchQuery, [Description("Amount of video matches to return")] int count = 1)
    {
        if (count > MAX_VIDEOS)
            return Result.FromError<string>($"The max allowed count is {MAX_VIDEOS}!");

        var sb = new StringBuilder();

        int i = 0;
        string? firstImage = null;

        await foreach (var video in _youtubeClient.Search.GetVideosAsync(searchQuery, CancellationToken))
        {
            if (i >= count) break;

            i++;
            sb.AppendLine($"{i}. {video.Title} | {video.Url} ({video.Duration})");
            firstImage ??= video.Thumbnails.FirstOrDefault()?.Url;
        }

        Embed embed;
        if (firstImage is null)
        {
            embed = new Embed(Colour: Color.Green,
                Title: $"YouTube Videos: {searchQuery}",
                Description: sb.ToString());
        }
        else
        {
            embed = new Embed(Colour: Color.Green,
                Title: $"YouTube Videos: {searchQuery}",
                Description: sb.ToString(),
                Image: new Optional<IEmbedImage>(new EmbedImage(firstImage)));
        }

        var reply = await _feedbackService.SendContextualEmbedAsync(embed, ct: CancellationToken);

        return reply.IsSuccess
            ? Result.FromSuccess()
            : Result.FromError(reply);
    }
}
