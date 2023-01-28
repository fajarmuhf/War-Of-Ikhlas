VAR name = "Budi"
VAR myName = "Kakek Ikhlas"
VAR QuestLevel = 1
VAR KillIfrit = 5
VAR KillIfritProgress = 0
VAR takeQuest = 1
VAR introQuest = 0
VAR rewardItem = ()
VAR jumlahRewardItem = 0

EXTERNAL takeQuestNow(level,reward)
EXTERNAL giveRewardQuest(level)

LIST ItemsRewardList = 1_1_1_apple

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
- 1 : -> RLevel1
- 2 : -> RLevel2
}
-> END
=== RLevel1 ===
~ rewardItem = (1_1_1_apple)
{ takeQuest:
- 1: ->RewardLevel1
- else: ->FinishQuest
}
=== RewardLevel1 ===
~ jumlahRewardItem = LIST_COUNT(rewardItem)
~ giveRewardQuest(QuestLevel)
->ContinueDoneQuest

=== RLevel2 ===
~ rewardItem = (1_1_1_apple)
{ takeQuest:
- 1: ->RewardLevel1
- else: ->FinishQuest
}
=== RewardLevel2 ===
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
* Yes, i can -> DoneQuest 
* No, may be latter -> END

=== Quest2 ===
~ KillIfrit = 10
Can you kill {KillIfrit} ifrit and tell to us if you done ?.
* Yes, i can -> DoneQuest 
* No, may be latter -> END

=== FinishQuest ===
~ takeQuestNow(QuestLevel,rewardItem)
-> END

=== function takeQuestNow(level,reward) ===
// Usually external functions can only return placeholder
// results, otherwise they'd be defined in ink!
~ return 1

=== function giveRewardQuest(level) ===
// Usually external functions can only return placeholder
// results, otherwise they'd be defined in ink!
~ return 1