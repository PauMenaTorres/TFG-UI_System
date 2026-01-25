// Fill out your copyright notice in the Description page of Project Settings.


#include "MainMenu.h"
#include "Components/Button.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetSystemLibrary.h"

void UMainMenu::NativeConstruct()
{
    Super::NativeConstruct();

    if(startButton)
    {
        startButton->OnClicked.AddDynamic(this, &UMainMenu::OnStartClicked);
    }
    if(optionsButton)
    {
        optionsButton->OnClicked.AddDynamic(this, &UMainMenu::OnOptionsClicked);
    }
    if(exitButton)
    {
        exitButton->OnClicked.AddDynamic(this, &UMainMenu::OnExitClicked);
    }
}


void UMainMenu::OnStartClicked()
{
    UE_LOG(LogTemp, Warning, TEXT("Start Clicked"));
    
    RemoveFromParent();
    
    delete this;
    
    UGameplayStatics::OpenLevel(GetWorld(), FName("LevelTest"));
}

void UMainMenu::OnOptionsClicked()
{
    // TODO: Define OptionsWidgetClass as a property
    // UUserWidget* OptionsWidget = CreateWidget<UUserWidget>(GetWorld(), OptionsWidgetClass);
    // if (OptionsWidget)
    // {
    //     OptionsWidget->AddToViewport();
    // }

    UE_LOG(LogTemp, Warning, TEXT("Options Clicked"));
}

void UMainMenu::OnExitClicked()
{
    UKismetSystemLibrary::QuitGame(this, nullptr, EQuitPreference::Quit, false);
    UE_LOG(LogTemp, Warning, TEXT("Exit Clicked"));
}
