#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static StreamFeedBot.Utils;

namespace StreamFeedBot.Rulesets
{
	public class RenegadePlatinumRuleset : Ruleset
	{
		public RenegadePlatinumRuleset(Memory memory, Settings settings)
			: base(memory, settings)
		{ }

		private static readonly uint[] BallIds =
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 499, 500, 576, 851
		};

		private static readonly int[] SpecialClasses_Renegade =
		{
			000, //PkMn Trainer Lucas
			001, //PkMn Trainer Dawn
			062, //Leader Roark
			063, //PkMn Trainer Barry (Cedric)
			064, //Leader Byron
			065, //Elite Four Aaron
			066, //Elite Four Bertha
			067, //Elite Four Flint
			068, //Elite Four Lucian
			069, //Champion Cynthia
			072, //Commander Mars
			074, //Leader Gardenia
			075, //Leader Wake
			076, //Leader Maylene
			077, //Leader Fatina
			078, //Leader Candice
			079, //Leader Volkner
			086, //Galactic Boss Cyrus
			087, //Commander Jupiter
			088, //Commander Saturn
			090, //PkMn Trainer Cheryl
			091, //PkMn Trainer Riley
			092, //PkMn Trainer Marley
			093, //PkMn Trainer Buck
			094, //PkMn Trainer Mira
			095, //PkMn Trainer Lucas
			096, //PkMn Trainer Dawn
			097, //Tower Tycoon Palmer
			099, //Hall Matron Argenta
			100, //Factory Head Thorton
			101, //Arcade Star Dahlia
			102, //Castle Valet Darach
		};

		private static readonly int[] SpecialIds_Renegade =
		{
			1029, //Bird Keeper Falkner
			1030, //Bug Catcher Bugsy
			1031, //Idol Whitney
			1032, //Ace Trainer Morty
			1033, //Black Belt Chuck
			1034, //Lass Jasmine
			1035, //Veteran Pryce
			1036, //Ace Trainer Clair
			1053, //Ace Trainer Dray
		};

		private readonly List<string> _badges = new List<string> { "Coal", "Forest", "Cobble", "Fen", "Relic", "Mine", "Icicle", "Beacon" };

		public override List<string>? Badges => _badges;
		
		private bool InBattle;

		public override string? CalculateDeltas(RunStatus? status, RunStatus? oldStatus, out string? announcement, out bool ping)
		{
			StringBuilder builder = new StringBuilder();
			StringBuilder aBuilder = new StringBuilder();
			if (oldStatus == null || status == null)
			{
				announcement = null;
				ping = true;
				return null; //calculate deltas between two statuses, not just one
			}

			if ((string.IsNullOrWhiteSpace(oldStatus.Name) || oldStatus.Gender != status.Gender) && !string.IsNullOrWhiteSpace(status.Name))
			{
				string choice = status.Gender == Gender.Female ? "girl" : "boy";

				builder.Append($"**We are a {choice} named {status.Name}!** ");
			}

			if (string.IsNullOrWhiteSpace(oldStatus.RivalName) && !string.IsNullOrWhiteSpace(status.RivalName))
			{
				builder.Append($"**We name our rival {status.RivalName}!** ");
			}

			if (status.BattleKind != null && status.GameStats != null && oldStatus.GameStats != null &&
			    status.GameStats.BattlesFought != oldStatus.GameStats.BattlesFought)
			{
				switch (status.BattleKind)
				{
					case BattleKind.Wild:
						if (status.EnemyParty != null && status.EnemyParty.Count > 0 && status.EnemyParty[0] != null)
						{
							string[] rand1 =
							{
								"come across", "run into", "step on", "stumble upon", "encounter", "bump into",
								"run across"
							};
							string[] rand2 =
								{"Facing off against", "Battling", "Grappling", "Confronted by", "Wrestling"};
							string[] rand3 =
							{
								"picks a fight with", "engages", "thinks it can take", "crashes into", "smacks into",
								"collides with", "jumps", "ambushes", "attacks", "assaults"
							};
							string[] choice =
							{
								$"We {rand1[Random.Next(rand1.Length)]} a wild {status.EnemyParty[0].Species!.Name}. ",
								$"{rand2[Random.Next(rand2.Length)]} a wild {status.EnemyParty[0].Species!.Name}. ",
								$"A wild {status.EnemyParty[0].Species!.Name} {rand3[Random.Next(rand3.Length)]} us. "
							};
							string message = choice[Random.Next(choice.Length)];
							builder.Append(message);
						}

						EnemyName = null;
						InBattle = true;
						break;
					case BattleKind.Trainer:
						if (status.EnemyTrainers?.Count == 1)
						{
							if (status.EnemyTrainers?[0] != null)
							{
								Trainer trainer = status.EnemyTrainers[0];
								if (SpecialClasses_Renegade.Contains(trainer.ClassId) || SpecialIds_Renegade.Contains(trainer.Id))
								{
									builder.Append($"**VS {trainer.ClassName} {trainer.Name}!** ");
									int id = trainer.Id;
									if (Attempts.TryGetValue(id, out int val))
									{
										builder.Append($"Attempt #{val + 1}! ");
										Attempts.Remove(id);
										Attempts.Add(id, val + 1);
									}
									else
									{
										Attempts.Add(id, 1);
									}
									InBattle = true;

									break;
								}

								if (trainer.ClassId == -1)
								{
									string[] rand1 =
									{
										"come across", "run into", "step on", "stumble upon", "encounter", "bump into",
										"run across"
									};
									string[] rand2 =
										{"Facing off against", "Battling", "Grappling", "Confronted by", "Wrestling"};
									string[] rand3 =
									{
										"picks a fight with", "engages", "thinks it can take", "crashes into",
										"smacks into",
										"collides with", "jumps", "ambushes", "attacks", "assaults"
									};
									string[] choice =
									{
										$"We {rand1[Random.Next(rand1.Length)]} a wild {status.EnemyParty![0].Species!.Name}. ",
										$"{rand2[Random.Next(rand2.Length)]} a wild {status.EnemyParty[0].Species!.Name}. ",
										$"A wild {status.EnemyParty[0].Species!.Name} {rand3[Random.Next(rand3.Length)]} us. "
									};
									string message = choice[Random.Next(choice.Length)];
									builder.Append(message);
									InBattle = true;
									EnemyName = null;
									break;
								}

								string[] c1 = {"fight", "battle", "face off against"};
								string[] c2 = {"cheeky", "rogue", "roving", "wandering"};
								string[] c3 = {" wandering", "n eager"};
								string[] choices =
								{
									$"We {c1[Random.Next(c1.Length)]} a {c2[Random.Next(c2.Length)]} {trainer.ClassName}, named {trainer.Name}{(status.EnemyParty.Any(x => x.Active == true) ? $", and their {string.Join(", ", status.EnemyParty.Where(x => x.Active == true).Select(x => x.Species?.Name ?? ""))}" : "")}. ",
									$"We get spotted by a{c3[Random.Next(c3.Length)]} {trainer.ClassName} named {trainer.Name}, and begin a battle{(status.EnemyParty.Any(x => x.Active == true) ? $" against their {string.Join(", ", status.EnemyParty.Where(x => x.Active == true).Select(x => x.Species?.Name ?? ""))}" : "")}. ",
									$"{trainer.ClassName} {trainer.Name} picks a fight with us{(status.EnemyParty.Any(x => x.Active == true) ? $", using their {string.Join(", ", status.EnemyParty.Where(x => x.Active == true).Select(x => x.Species?.Name ?? ""))}" : "")}. "
								};
								builder.Append(choices[Random.Next(choices.Length)]);
							}
						}
						else if (status.EnemyTrainers?.Count == 2)
						{
							if (status.EnemyTrainers[0] != null && status.EnemyTrainers[1] != null)
							{
								Trainer trainer0 = status.EnemyTrainers[0];
								Trainer trainer1 = status.EnemyTrainers[1];

								if (((SpecialClasses_Renegade.Contains(trainer0.ClassId) || SpecialIds_Renegade.Contains(trainer0.Id)) ||
								     (SpecialClasses_Renegade.Contains(trainer1.ClassId) || SpecialIds_Renegade.Contains(trainer1.Id))) &&
								    trainer1.ClassId != 0)
								{
									builder.Append(
										$"**VS {trainer0.ClassName} {trainer0.Name} and {trainer1.ClassName} {trainer1.Name}!** ");
									if (SpecialClasses_Renegade.Contains(trainer0.ClassId) || SpecialIds_Renegade.Contains(trainer0.Id))
									{
										if (Attempts.TryGetValue(trainer0.Id, out int val))
										{
											builder.Append($"Attempt #{val + 1}! ");
											Attempts.Remove(trainer0.Id);
											Attempts.Add(trainer0.Id, val + 1);
										}
										else
										{
											Attempts.Add(trainer0.Id, 1);
										}
									}
									else
									{
										if (Attempts.TryGetValue(trainer1.Id, out int val))
										{
											builder.Append($"Attempt #{val + 1}! ");
											Attempts.Remove(trainer1.Id);
											Attempts.Add(trainer1.Id, val + 1);
										}
										else
										{
											Attempts.Add(trainer1.Id, 1);
										}
									}
								}
								else if (SpecialClasses_Renegade.Contains(trainer0.ClassId) || SpecialIds_Renegade.Contains(trainer0.Id))
								{
									builder.Append($"**VS {trainer0.ClassName}s {trainer0.Name}!** ");
									if (Attempts.TryGetValue(trainer0.Id, out int val))
									{
										builder.Append($"Attempt #{val + 1}! ");
										Attempts.Remove(trainer0.Id);
										Attempts.Add(trainer0.Id, val + 1);
									}
									else
									{
										Attempts.Add(trainer0.Id, 1);
									}
								}
								else if (trainer0.ClassId == -1 || trainer1.ClassId == -1)
								{
									string[] rand1 =
									{
										"come across", "run into", "step on", "stumble upon", "encounter",
										"bump into",
										"run across"
									};
									string[] rand2 =
									{
										"Facing off against", "Battling", "Grappling", "Confronted by", "Wrestling"
									};
									string[] rand3 =
									{
										"picks a fight with", "engages", "thinks it can take", "crashes into",
										"smacks into",
										"collides with", "jumps", "ambushes", "attacks", "assaults"
									};
									string[] choice =
									{
										$"We {rand1[Random.Next(rand1.Length)]} a wild {status.EnemyParty![0].Species!.Name}. ",
										$"{rand2[Random.Next(rand2.Length)]} a wild {status.EnemyParty[0].Species!.Name}. ",
										$"A wild {status.EnemyParty[0].Species!.Name} {rand3[Random.Next(rand3.Length)]} us. "
									};
									string message = choice[Random.Next(choice.Length)];
									builder.Append(message);
									EnemyName = null;
								}
								else if (trainer1.ClassId != 0)
								{
									string[] choices =
									{
										$"Both {trainer0.ClassName} {trainer0.Name} and {trainer1.ClassName} {trainer1.Name} challenge us to a battle at the same time!",
									};
									builder.Append(choices[Random.Next(choices.Length)]);
								}
								else
								{
									string[] choices =
									{
										$"{trainer0.ClassName} {trainer0.Name} challenge us to a battle at the same time!",
									};
									builder.Append(choices[Random.Next(choices.Length)]);
								}
							}
						}

						InBattle = true;
						break;
				}
			}
			
			if (status.GameStats != null && oldStatus.GameStats != null && status.GameStats.Blackouts > oldStatus.GameStats.Blackouts)
			{
				string[] options = { "**BLACKED OUT!** ", "**We BLACK OUT!** ", "**BLACK OUT...** " };
				string message = options[Random.Next(options.Length)];
				builder.Append(message);
				InBattle = false;
			}

			if (status.BattleKind == null && oldStatus!.BattleKind == BattleKind.Trainer &&
			    status.GameStats?.Blackouts == oldStatus.GameStats?.Blackouts && InBattle)
			{
				if (oldStatus.EnemyTrainers != null)
				{
					Trainer trainer = oldStatus.EnemyTrainers[0];
					if (SpecialClasses_Renegade.Contains(trainer.ClassId) || SpecialIds_Renegade.Contains(trainer.Id))
					{
						builder.Append($"**Defeated {trainer.ClassName} {trainer.Name}!** ");
						EnemyName = trainer.ClassName + " " + trainer.Name;
					}

					if (trainer.ClassId == 69) //Cynthia
					{
						builder.Append("**TEH URN!** ");
						if (!Memory.Urned)
						{
							aBuilder.Append($"**We defeated {trainer.ClassName} {trainer.Name}! TEH URN!** ");
							Memory.Urned = true;
						}
					}
				}
			}
			
			if (status.BattleKind == null && oldStatus!.BattleKind != null && InBattle)
			{
				InBattle = false;
			}

			if (status?.Badges != oldStatus?.Badges)
			{
				List<bool> gains = new List<bool>();
				int j = 0;
				foreach (bool badgeFlag in status!.BadgesFlags)
				{
					if (badgeFlag != oldStatus?.BadgesFlags[j] && badgeFlag)
					{
						gains.Add(true);
					}
					else
					{
						gains.Add(false);
					}
					j++;
				}

				for (int i = 0; i < gains.Count; i++)
				{
					if (gains[i])
					{
						string[] choices =
						{
							$"**Got the {_badges[i]} Badge!** ",
							$"**Received the {_badges[i]} Badge!** "
						};
						string choice = choices[Random.Next(choices.Length)];
						builder.Append(choice);
						if (!Memory.AnnouncedBadges.Contains((uint)i))
						{
							if (oldStatus!.BattleKind == BattleKind.Trainer)
							{
								aBuilder.Append(
									$"**We defeated {oldStatus.EnemyTrainers![0].ClassName} {oldStatus.EnemyTrainers![0].Name} and received the {_badges[i]} badge!** ");
								Memory.AnnouncedBadges.Add((uint)i);
							}
							else if (EnemyName != null)
							{
								aBuilder.Append(
									$"**We defeated {EnemyName} and received the {_badges[i]} badge!** ");
								EnemyName = null;
								Memory.AnnouncedBadges.Add((uint)i);
							}
							else
							{
								aBuilder.Append(
									$"**We received the {_badges[i]} badge!** ");
								Memory.AnnouncedBadges.Add((uint)i);
							}
						}
					}
				}
			}

			List<uint> ids = new List<uint>();

			ItemEqualityComparer comparer = new ItemEqualityComparer();

			List<Item> distinctItems = new List<Item>();
			if (status?.Items?.Items != null)
				distinctItems.AddRange(status.Items.Items);
			if (oldStatus?.Items?.Items != null)
				distinctItems.AddRange(oldStatus.Items.Items);
			distinctItems = distinctItems.Distinct(comparer).ToList();

			foreach (Item item in distinctItems)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Items?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Items?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Items?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctMedicine = new List<Item>();
			if (status?.Items?.Medicine != null)
				distinctMedicine.AddRange(status.Items.Medicine);
			if (oldStatus?.Items?.Medicine != null)
				distinctMedicine.AddRange(oldStatus.Items.Medicine);
			distinctMedicine = distinctMedicine.Distinct(comparer).ToList();

			foreach (Item item in distinctMedicine)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Medicine?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Medicine?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Medicine?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctBalls = new List<Item>();
			if (status?.Items?.Balls != null)
				distinctBalls.AddRange(status.Items.Balls);
			if (oldStatus?.Items?.Balls != null)
				distinctBalls.AddRange(oldStatus.Items.Balls);
			distinctBalls = distinctBalls.Distinct(comparer).ToList();

			foreach (Item item in distinctBalls)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Balls?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Balls?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Balls?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (status.BattleKind == BattleKind.Wild && status.EnemyParty != null && status.EnemyParty.Count > 0 && count < 0 && BallIds.Contains(item.Id))
						builder.Append(
							$"We throw {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"some {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")} at the wild {status.EnemyParty[0].Species!.Name}. ");
					else if (status.BattleKind == BattleKind.Trainer && status.EnemyParty != null && status.EnemyParty.Count > 0 && count < 0 && BallIds.Contains(item.Id))
						builder.Append(
							$"We throw {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"some {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")} at the opponent's {status.EnemyParty[0].Species!.Name}. ");
					else if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctTMs = new List<Item>();
			if (status?.Items?.TMs != null)
				distinctTMs.AddRange(status.Items.TMs);
			if (oldStatus?.Items?.TMs != null)
				distinctTMs.AddRange(oldStatus.Items.TMs);
			distinctTMs = distinctTMs.Distinct(comparer).ToList();

			foreach (Item item in distinctTMs)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.TMs?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.TMs?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.TMs?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctBerries = new List<Item>();
			if (status?.Items?.Berries != null)
				distinctBerries.AddRange(status.Items.Berries);
			if (oldStatus?.Items?.Berries != null)
				distinctBerries.AddRange(oldStatus.Items.Berries);
			distinctBerries = distinctBerries.Distinct(comparer).ToList();

			foreach (Item item in distinctBerries)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Berries?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Berries?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Berries?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctMail = new List<Item>();
			if (status?.Items?.Mail != null)
				distinctMail.AddRange(status.Items.Mail);
			if (oldStatus?.Items?.Mail != null)
				distinctMail.AddRange(oldStatus.Items.Mail);
			distinctMail = distinctMail.Distinct(comparer).ToList();

			foreach (Item item in distinctMail)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Mail?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Mail?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Mail?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctBattleItems = new List<Item>();
			if (status?.Items?.BattleItems != null)
				distinctBattleItems.AddRange(status.Items.BattleItems);
			if (oldStatus?.Items?.BattleItems != null)
				distinctBattleItems.AddRange(oldStatus.Items.BattleItems);
			distinctBattleItems = distinctBattleItems.Distinct(comparer).ToList();

			foreach (Item item in distinctBattleItems)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.BattleItems?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.BattleItems?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.BattleItems?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			List<Item> distinctKey = new List<Item>();
			if (status?.Items?.Key != null)
				distinctKey.AddRange(status.Items.Key);
			if (oldStatus?.Items?.Key != null)
				distinctKey.AddRange(oldStatus.Items.Key);
			distinctKey = distinctKey.Distinct(comparer).ToList();

			foreach (Item item in distinctKey)
			{
				if (item.Id == 0) continue;
				if (ids.Contains(item.Id)) continue;
				long count = status?.Items?.Key?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1) ?? 0;
				bool res = oldStatus?.Items?.Key?.FirstOrDefault(x => x.Id == item.Id) != null;
				if (res)
				{
					long? oldCount = oldStatus?.Items?.Key?.Where(x => x.Id == item.Id)?.Sum(x => x.Count ?? 1);
					count -= oldCount ?? 0;
				}

				if (count != 0)
				{
					Pokemon[] monsGive = status!.Party.Where(x => x != null).Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							(oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem == null ||
							 oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id !=
							 x.HeldItem?.Id))
						.ToArray();
					Pokemon[] monsTake = status.Party.Where(x => x != null).Where(x =>
							x.HeldItem == null ||
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => x.PersonalityValue == y.PersonalityValue).HeldItem?.Id !=
							x.HeldItem.Id)
						.Where(x =>
							oldStatus!.Party.Where(y => y != null).Any(y => x.PersonalityValue == y.PersonalityValue) &&
							oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem != null
							&& oldStatus.Party.Where(y => y != null).First(y => y.PersonalityValue == x.PersonalityValue).HeldItem?.Id ==
							item.Id)
						.ToArray();
					List<Pokemon> monsGivePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsGivePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem != null && x.HeldItem.Id == item.Id)
								.Where(x => oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											(oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem == null ||
											 oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
												 .HeldItem?.Id != x.HeldItem?.Id)).ToList());
						}
					}

					List<Pokemon> monsTakePc = new List<Pokemon>();
					foreach (Box box in status.PC?.Boxes ?? new List<Box>())
					{
						Box? oldBox = oldStatus!.PC?.Boxes?.FirstOrDefault(x => x.BoxNumber == box.BoxNumber);
						if (oldBox != null)
						{
							monsTakePc.AddRange(box.BoxContents
								.Where(x => x.HeldItem == null ||
											oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
											oldBox.BoxContents.First(y => x.PersonalityValue == y.PersonalityValue)
												.HeldItem?.Id != x.HeldItem?.Id).Where(x =>
									oldBox.BoxContents.Any(y => x.PersonalityValue == y.PersonalityValue) &&
									oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue).HeldItem !=
									null && oldBox.BoxContents.First(y => y.PersonalityValue == x.PersonalityValue)
										.HeldItem?.Id == item.Id).ToList());
						}
					}
					if (monsGive.Length != 0)
					{
						foreach (Pokemon mon in monsGive)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsGivePc != null && monsGivePc.Count != 0)
					{
						foreach (Pokemon mon in monsGivePc)
						{
							builder.Append($"We give {mon.Name} ({mon.Species!.Name}) {IndefiniteArticle(item.Name)} {item.Name} to hold. ");
							count++;
						}
					}

					if (monsTake.Length != 0)
					{
						foreach (Pokemon mon in monsTake)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (monsTakePc != null && monsTakePc.Count != 0)
					{
						foreach (Pokemon mon in monsTakePc)
						{
							builder.Append($"We take {IndefiniteArticle(item.Name)} {item.Name} away from {mon.Name} ({mon.Species!.Name}). ");
							count--;
						}
					}

					if (count < 0 && status.Money > oldStatus!.Money && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
					{
						builder.Append(
							$"We sell {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count < 0)
						builder.Append(
							$"We use {(count == -1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					else if (count > 0 && status.Money < oldStatus!.Money)
					{
						builder.Append(
							$"We buy {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
					}
					else if (count > 0)
						builder.Append(
							$"We pick up {(count == 1 ? $"{IndefiniteArticle(item.Name)} {item.Name}" : $"{Math.Abs(count)} {(item.Name?.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) == true ? item.Name : item.Name + "s")}")}. ");
				}

				ids.Add(item.Id);
			}

			if ((status!.Money < oldStatus!.Money || status.Money > oldStatus.Money) && (oldStatus.BattleKind == null || oldStatus.BattleKind == BattleKind.None))
				builder.Append($"We have ₽{status.Money} left. ");

			if (status.Party != null)
			{
				for (int i = 0; i < status.Party.Count; i++)
				{
					Pokemon? oldMon = i >= oldStatus?.Party?.Count ? null : oldStatus?.Party?[i];
					if (oldMon == null) continue;
					uint pv = oldMon.PersonalityValue;
					Pokemon? mon = status.Party.FirstOrDefault(x => x.PersonalityValue == pv);

					if (mon == null)
					{
						continue;
					}

					if (mon.Level != oldMon.Level)
					{
						string[] choices =
						{
							$"**{oldMon.Name} ({oldMon.Species!.Name}) has grown to level {mon.Level}!** ",
							$"**{oldMon.Name} ({oldMon.Species!.Name}) is now level {mon.Level}!** ",
							$"**{oldMon.Name} ({oldMon.Species!.Name}) has leveled up to level {mon.Level}!** "
						};
						string message = choices[Random.Next(choices.Length)];
						builder.Append(message);
					}

					if (mon.Species!.NationalDex != oldMon.Species!.NationalDex)
					{
						string[] choices =
						{
							$"**{oldMon.Name} ({oldMon.Species.Name}) has evolved into {IndefiniteArticle(mon.Species.Name)} {mon.Species.Name}!** ",
							$"**{oldMon.Name} ({oldMon.Species.Name}) evolves into {IndefiniteArticle(mon.Species.Name)} {mon.Species.Name}!** "
						};
						string message = choices[Random.Next(choices.Length)];
						builder.Append(message);
					}
				}
			}

			if (status.Party != null)
			{
				foreach (Pokemon mon in status.Party)
				{
					if (mon == null) continue;
					uint pv = mon.PersonalityValue;
					Pokemon? oldMon = oldStatus?.Party?.Where(x => x != null)?.FirstOrDefault(x => x.PersonalityValue == pv);

					if (oldMon == null) continue;
					foreach (Move? move in mon.Moves!)
					{
						if (move == null) continue;
						if (!oldMon.Moves.Where(x => x != null).Select(x => x.Id).Contains(move.Id))
						{
							if (oldMon.Moves!.Count == 4)
							{
								Move? oldMove;
								if (!oldMon.Moves.Where(x => x != null).Select(x => x.Id).Contains(move.Id) &&
									oldMon.Moves.Count == 4)
								{
									oldMove = oldMon.Moves.First(x => !mon.Moves.Select(y => y.Id).Contains(x.Id));
								}
								else
									oldMove = null;

								builder.Append(
									oldMove != null
										? $"**{mon.Name} ({mon.Species!.Name}) learned {move.Name} over {oldMove.Name}!** "
										: $"**{mon.Name} ({mon.Species!.Name}) learned {move.Name}!** ");
							}
							else
							{
								builder.Append($"**{mon.Name} ({mon.Species!.Name}) learned {move.Name}!** ");
							}
						}
					}

					if (mon.Health![0] == 0 && oldMon.Health![0] != 0)
					{
						string[] choice =
						{
							$"**{oldMon.Name} ({oldMon.Species!.Name}) has fainted!** ",
							$"**{oldMon.Name} ({oldMon.Species.Name}) has fallen!** ",
						};
						builder.Append(choice[Random.Next(choice.Length)]);
					}
				}
			}

			if (status.Daycare != null)
			{
				foreach (Pokemon mon in status.Daycare)
				{
					if (oldStatus?.Daycare?.Any(x => x.PersonalityValue == mon.PersonalityValue) == true)
						continue;

					string[] choices =
					{
						$"**We drop {mon.Name} ({mon.Species!.Name}) off at the daycare.** ",
						$"**We put {mon.Name} ({mon.Species.Name}) into the daycare.** ",
						$"**We dump {mon.Name} ({mon.Species.Name}) into the daycare.** ",
						$"**We leave {mon.Name} ({mon.Species.Name} at the daycare centre.** "
					};
					builder.Append(choices[Random.Next(choices.Length)]);
				}
			}

			if (oldStatus?.Daycare != null)
			{
				foreach (Pokemon mon in oldStatus.Daycare)
				{
					if (status.Daycare.Any(x => x.PersonalityValue == mon.PersonalityValue))
						continue;

					string[] choices =
					{
						$"**We take {mon.Name} ({mon.Species!.Name}) back from the daycare.** ",
						$"**We pick up {mon.Name} ({mon.Species.Name}) from the daycare.** "
					};
					builder.Append(choices[Random.Next(choices.Length)]);
				}
			}

			if (status.Party != null)
			{
				foreach (Pokemon mon in status.Party)
				{
					if (mon == null) continue;
					uint pv = mon.PersonalityValue;

					List<uint> values =
						oldStatus?.Party?.Where(x => x != null)?.Select(x => x.PersonalityValue)?
							.ToList() ?? new List<uint>();
					List<uint> boxValues = new List<uint>();
					List<Pokemon> pokemon = new List<Pokemon>();
					foreach (List<Pokemon>? p in oldStatus?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
					{
						if (p != null)
							pokemon.AddRange(p);
					}

					values.AddRange(pokemon.Select(x => x.PersonalityValue));
					boxValues.AddRange(pokemon.Select(x => x.PersonalityValue));
					if (oldStatus?.Daycare != null)
					{
						values.AddRange(oldStatus.Daycare.Where(x => x != null).Select(x => x.PersonalityValue));
					}

					if (ReleasedDictionary.Any(x => x.Key.PersonalityValue == mon.PersonalityValue))
					{
						string[] choices =
						{
							$"**We retrieve {mon.Name} ({mon.Species!.Name}) from the PC!** ",
							$"**We withdraw {mon.Name} ({mon.Species.Name}) from the PC!** "
						};
						builder.Append(choices[Random.Next(choices.Length)]);
						Pokemon temp = ReleasedDictionary.First(x => x.Key.PersonalityValue == mon.PersonalityValue)
							.Key;
						ReleasedDictionary.Remove(temp);
					}
					else if (!values.Contains(pv))
					{
						builder.Append(
							$"**Caught a {(mon.Gender != null ? Enum.GetName(typeof(Gender), mon.Gender) + " " : "")}Lv. {mon.Level} {mon.Species!.Name}!** {(mon.Name == mon.Species.Name ? "No nickname. " : $"Nickname: `{mon.Name}`. ")}");
						if (status.Party.All(x => x.PersonalityValue != pv))
						{
							builder.Append("Sent to the PC. ");
						}
					}
					else if (boxValues.Contains(pv))
					{
						string[] choices =
						{
							$"**We retrieve {mon.Name} ({mon.Species!.Name}) from the PC!** ",
							$"**We withdraw {mon.Name} ({mon.Species.Name}) from the PC!** "
						};
						builder.Append(choices[Random.Next(choices.Length)]);
					}
				}
			}

			if (status?.PC?.Boxes != null && oldStatus?.Party != null && oldStatus.Party.Count != 0)
			{
				foreach (Box box in status.PC.Boxes)
				{
					foreach (Pokemon? mon in box.BoxContents!)
					{
						if (mon == null) continue;
						uint pv = mon.PersonalityValue;
						List<uint> values =
							oldStatus?.Party?.Where(x => x != null)?.Select(x => x.PersonalityValue)?.ToList() ?? new List<uint>();
						List<uint> boxValues = new List<uint>();
						List<Pokemon> pokemon = new List<Pokemon>();
						foreach (List<Pokemon>? p in oldStatus?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
						{
							if (p != null)
								pokemon.AddRange(p);
						}

						values.AddRange(pokemon.Select(x => x.PersonalityValue));
						boxValues.AddRange(pokemon.Select(x => x.PersonalityValue));
						if (oldStatus?.Daycare != null)
						{
							values.AddRange(oldStatus.Daycare.Where(x => x != null).Select(x => x.PersonalityValue));
						}

						if (ReleasedDictionary.Any(x => x.Key.PersonalityValue == mon.PersonalityValue))
						{
							string[] choices =
							{
								$"**We deposited {mon.Name} ({mon.Species!.Name}) in the PC!** ",
								$"**We put {mon.Name} ({mon.Species.Name}) in the PC!** ",
								$"**Deposited {mon.Name} ({mon.Species.Name}) in the PC!** "
							};
							builder.Append(choices[Random.Next(choices.Length)]);
							Pokemon temp = ReleasedDictionary.First(x => x.Key.PersonalityValue == mon.PersonalityValue)
								.Key;
							ReleasedDictionary.Remove(temp);
						}
						else if (!values.Contains(pv))
						{
							builder.Append(
								$"**Caught a {(mon.Gender != null ? Enum.GetName(typeof(Gender), mon.Gender) + " " : "")}Lv. {mon.Level} {mon.Species!.Name}!** {(mon.Name == mon.Species.Name ? "No nickname. " : $"Nickname: `{mon.Name}`. ")}");
							builder.Append($"Sent to Box {box.BoxNumber}. ");
						}
					}
				}
			}

			if (oldStatus?.PC != null)
			{
				List<Pokemon> oldBoxedMons = new List<Pokemon>();
				foreach (List<Pokemon>? p in oldStatus?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
				{
					if (p != null)
						oldBoxedMons.AddRange(p);
				}

				List<Pokemon> newBoxedMons = new List<Pokemon>();
				foreach (List<Pokemon>? p in status?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
				{
					if (p != null)
						newBoxedMons.AddRange(p);
				}

				foreach (Pokemon mon in newBoxedMons)
				{
					if (oldStatus?.Party?.Where(x => x != null)?.Any(x => x.PersonalityValue == mon.PersonalityValue) == true ||
						ReleasedDictionary.Any(x => x.Key.PersonalityValue == mon.PersonalityValue) &&
						oldBoxedMons.All(x => x.PersonalityValue != mon.PersonalityValue))
					{
						string[] choices =
						{
							$"**We deposited {mon.Name} ({mon.Species!.Name}) in the PC!** ",
							$"**We put {mon.Name} ({mon.Species.Name}) in the PC!** ",
							$"**Deposited {mon.Name} ({mon.Species.Name}) in the PC!** "
						};
						builder.Append(choices[Random.Next(choices.Length)]);
					}
					if (ReleasedDictionary.Any(x => x.Key.PersonalityValue == mon.PersonalityValue))
					{
						Pokemon temp =
							ReleasedDictionary.First(x => x.Key.PersonalityValue == mon.PersonalityValue).Key;
						ReleasedDictionary.Remove(temp);
					}
				}

				foreach (Pokemon oldMon in oldBoxedMons)
				{
					if ((status?.Party?.Where(x => x != null)?.All(x => x.PersonalityValue != oldMon.PersonalityValue) ?? true) &&
						newBoxedMons.All(x => x.PersonalityValue != oldMon.PersonalityValue) &&
						(status?.Daycare?.Where(x => x != null)?.All(x => x.PersonalityValue != oldMon.PersonalityValue) ?? true))
					{
						if (ReleasedDictionary.All(x => x.Key.PersonalityValue != oldMon.PersonalityValue))
							ReleasedDictionary.Add(oldMon, 1);
					}
				}
			}

			if (oldStatus?.Party != null)
			{
				foreach (Pokemon mon in oldStatus.Party)
				{
					if (mon == null) continue;

					List<Pokemon> newBoxedMons = new List<Pokemon>();
					foreach (List<Pokemon>? p in status?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
					{
						if (p != null)
							newBoxedMons.AddRange(p);
					}

					if ((status?.Party?.Where(x => x != null)?.All(x => x.PersonalityValue != mon.PersonalityValue) ?? true) &&
						newBoxedMons.All(x => x.PersonalityValue != mon.PersonalityValue) &&
						(status?.Daycare?.Where(x => x != null)?.All(x => x.PersonalityValue != mon.PersonalityValue) ?? true))
					{
						if (ReleasedDictionary.All(x => x.Key.PersonalityValue != mon.PersonalityValue))
							ReleasedDictionary.Add(mon, 1);
					}
				}
			}
			Dictionary<Pokemon, int> releasedCopy = new Dictionary<Pokemon, int>(ReleasedDictionary);
			foreach ((Pokemon mon, int time) in releasedCopy)
			{
				List<uint> values = status?.Party?.Where(x => x != null)
					?.Select(x => x.PersonalityValue)
					?.ToList() ?? new List<uint>();

				List<Pokemon> mons = new List<Pokemon>();
				foreach (List<Pokemon>? boxedMons in status?.PC?.Boxes?.Where(x => x?.BoxContents != null)?.Select(x => x.BoxContents) ?? new List<List<Pokemon>>())
				{
					if (boxedMons != null)
						mons.AddRange(boxedMons);
				}
				values.AddRange(mons.Select(x => x.PersonalityValue));
				if (oldStatus?.Daycare != null)
				{
					values.AddRange(oldStatus.Daycare.Where(x => x != null).Select(x => x.PersonalityValue));
				}

				if (!values.Contains(mon.PersonalityValue))
				{
					if (time == 6)
					{
						string[] choices =
						{
							$"**WE RELEASE {mon.Name} ({mon.Species!.Name})!** ",
							$"**{mon.Name} ({mon.Species.Name}) HAS BEEN RELEASED! BYE {mon.Name!.ToUpperInvariant()}!** "
						};
						builder.Append(choices[Random.Next(choices.Length)]);
						ReleasedDictionary.Remove(mon);
					}
					else
					{
						ReleasedDictionary[mon] = time + 1;
					}
				}
			}

			if (status?.MapId != oldStatus?.MapId)
			{
				if (status?.MapId == 176 || (status?.MapId == 177 && oldStatus?.MapId != 176))
				{
					List<string> options = new List<string>
					{
						$"**We're locked into the E4 for {(Memory.Urned ? "Rematch " : "")}Attempt #{(Memory.Urned ? Memory.E4RematchNum : Memory.E4AttemptNum)}!** ",
						$"**We're in for E4 {(Memory.Urned ? "Rematch " : "")}Attempt #{(Memory.Urned ? Memory.E4RematchNum : Memory.E4AttemptNum)}!** ",
						$"**Welcome back to the E4! {(Memory.Urned ? "Rematch " : "")}Attempt #{(Memory.Urned ? Memory.E4RematchNum : Memory.E4AttemptNum)}!** ",
						$"**The door slams shut behind us! E4 {(Memory.Urned ? "Rematch " : "")}Attempt #{(Memory.Urned ? Memory.E4RematchNum : Memory.E4AttemptNum)}!** ",
						$"**We stroll boldly into the E4 chambers and are locked inside! {(Memory.Urned ? "Rematch " : "")}Attempt #{(Memory.Urned ? Memory.E4RematchNum : Memory.E4AttemptNum)}!** "
					};
					string message = options[Random.Next(options.Count)];
					builder.Append(message);
					if (Memory.Urned) Memory.E4RematchNum++;
					else Memory.E4AttemptNum++;
				}
				else if (!string.IsNullOrWhiteSpace(status?.MapName) && status?.MapName != oldStatus?.MapName)
				{
					string[] move = { "head", "go", "step", "move", "travel", "walk", "stroll", "stride" };
					string choice = move[Random.Next(move.Length)];
					List<string> options = new List<string>
					{
						$"{status!.MapName}. ", $"In {status.MapName}. ",
						$"Now in {status.MapName}. ",
						$"We {choice} into {status.MapName}. ",
						$"Arrived at {status.MapName}. "
					};
					string message = options[Random.Next(options.Count)];
					builder.Append(message);
				}
			}

			announcement = aBuilder.ToString().Length == 0 ? null : aBuilder.ToString();
			ping = true;
			return builder.ToString().Length == 0 ? null : builder.ToString();
		}
	}
}
