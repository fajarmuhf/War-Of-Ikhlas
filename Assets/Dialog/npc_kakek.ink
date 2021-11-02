VAR name = "Budi"
VAR myName = "Kakek Ikhlas"
VAR QuestLevel = 1
VAR KillIfrit = 5
VAR KillIfritProgress = 0
VAR takeQuest = 0
VAR introQuest = 1
VAR rewardItem = ()
VAR jumlahRewardItem = 0

EXTERNAL takeQuestNow(level)
EXTERNAL giveRewardQuest(level)

LIST ItemsRewardList = 1_shampoo, 2_sabun, 3_apple, 1_sikatGigi

{ introQuest:
- 0: -> NoIntruduction
- 1: -> Introduction
}
=== Introduction ===
Hello {name}, Welcome to Islamic Island.My name is {myName}. -> NoIntruduction

=== NoIntruduction ===
{ takeQuest:
- 0: -> LoadQuest
- 1: -> ProgressQuest
}

=== ProgressQuest ===
{ KillIfrit:
- KillIfritProgress: -> DoneQuest
- else: -> NotYetQuest
}

=== DoneQuest ===
{ QuestLevel:
- 1 : -> RewardLevel1
- 2 : -> RewardLevel2
}
->END

=== RewardLevel1 ===
~ rewardItem = (1_shampoo,2_sabun,1_sikatGigi)
~ jumlahRewardItem = LIST_COUNT(rewardItem)
~ giveRewardQuest(QuestLevel)
->ContinueDoneQuest

=== RewardLevel2 ===
~ rewardItem = (2_sabun)
~ jumlahRewardItem = LIST_COUNT(rewardItem)
~ giveRewardQuest(QuestLevel)
->ContinueDoneQuest

=== ContinueDoneQuest ===
VAR currentList = ()
 
VAR index = 1

~ currentList = LIST_RANGE(rewardItem,index,index)

Conratulation you have finished job,this is reward for you.
->LoopItem

=== LoopItem ===
{ index > LIST_VALUE(LIST_MAX(rewardItem)): -> LoadQuest}
//~ temp quantityRewardItem = LIST_VALUE(currentList)
~temp hasil = currentList

{ currentList != (): (You received {hasil}) }

~index++
~ currentList = LIST_RANGE(rewardItem,index,index)

-> LoopItem

=== NotYetQuest ===
you haven't finished the quest.Please finish quest
    * Ok,see you letter -> END
file:///Applications/Inky.app/Contents/Resources/app.asar/renderer/index.html#
=== LoadQuest ===
Will you take some job to help this Island ?. 

 * Yes, i will help now --> Quest
 * No, may be latter
    -> END

=== Quest ===
{ QuestLevel:
- 1: -> Quest1
- 2: -> Quest2
}

=== Quest1 ===
~ KillIfrit = 5
Can you kill {KillIfrit} ifrit and tell to us if you done ?.
* Yes, i can
 ~ takeQuestNow(QuestLevel)
-> END
* No, may be latter
-> END

=== Quest2 ===
~ KillIfrit = 10
Can you kill {KillIfrit} ifrit and tell to us if you done ?.
* Yes, i can
 ~ takeQuestNow(QuestLevel)
-> END
* No, may be latter
-> END

=== function takeQuestNow(level) ===
// Usually external functions can only return placeholder
// results, otherwise they'd be defined in ink!
~ return 1

=== function giveRewardQuest(level) ===
// Usually external functions can only return placeholder
// results, otherwise they'd be defined in ink!
~ return 1