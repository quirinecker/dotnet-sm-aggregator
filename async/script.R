print_language_stat <- function(game) {
  print(game)
  frame <- read.csv(
    paste("./csv/", game, "_language_stats.csv", sep = ""),
    header = TRUE, sep = ";",
    stringsAsFactors = FALSE
  )

  frame <- frame[order(frame$Count, decreasing = TRUE), ]

  png(paste("./statistics/languages", game, "_language_stats.png", sep = ""))
  barplot(frame$Count, names.arg = frame$Language)
  print(frame)
  dev.off()
}

print_language_stats <- function() {
  print_language_stat("Overwatch_2")
  print_language_stat("Call_of_Duty:_Warzone")
  print_language_stat("Clair_Obscur:_Expedition_33")
  print_language_stat("Counter-Strike")
  print_language_stat("Dead_by_Daylight")
  print_language_stat("Dota_2")
  print_language_stat("Escape_from_Tarkov")
  print_language_stat("Fortnite")
  print_language_stat("Grand_Theft_Auto_V")
  print_language_stat("Hearthstone")
  print_language_stat("IRL")
  print_language_stat("Just_Chatting")
  print_language_stat("League_of_Legends")
  print_language_stat("Minecraft")
  print_language_stat("Overwatch_2")
  print_language_stat("Rust")
  print_language_stat("Street_Fighter_6")
  print_language_stat("Teamfight_Tactics")
  print_language_stat("VALORANT")
  print_language_stat("World_of_Warcraft")
}

print_stream_stats <- function() {
  frame <- read.csv(
    "./csv/stream_stats.csv",
    header = TRUE, sep = ";",
    stringsAsFactors = FALSE
  )

  print(frame)
  print(as.numeric(frame[0, ]))

  png(paste("./statistics/streams.png", sep = ""))
  pie(as.numeric(frame[1, ]), labels = c("normal", "mature"))
  print(frame)
  dev.off()
}

print_time_stats <- function() {
  frame <- read.csv(
    "./csv/stream_time_stats.csv",
    header = TRUE, sep = ";",
    stringsAsFactors = FALSE
  )

  head_frame <- head(frame[order(frame$Time, decreasing = TRUE), ], n = 10)

  png(paste("./statistics/time.png", sep = ""))
  par(mar = c(5, 10, 4, 2)) # Increase the left margin (second value)
  barplot(head_frame$Time, names.arg = head_frame$Streamer, horiz = TRUE, las = 2)
  dev.off()
}

print_streamer_type_stats <- function() {
  frame <- read.csv(
    "./csv/streamer_type_stats.csv",
    header = TRUE, sep = ";",
    stringsAsFactors = FALSE
  )

  print(frame)

  png(paste("./statistics/streamer_type.png", sep = ""))
  pie(as.numeric(frame[1, ]), labels = colnames(frame))
  dev.off()
}


print_streamer_type_stats()

# print_time_stats()
# print_language_stats()
# print_stream_stats()
