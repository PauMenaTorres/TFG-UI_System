// Fill out your copyright notice in the Description page of Project Settings.


#include "Player_Controller.h"
#include "Blueprint/UserWidget.h"

void APlayer_Controller::BeginPlay()
{
    Super::BeginPlay();

    UE_LOG(LogTemp, Warning, TEXT("=== Player_Controller BeginPlay ==="));

    // Cargar el widget directamente por ruta
    UClass* WidgetClass = LoadClass<UUserWidget>(nullptr, TEXT("/Game/UI/MainMenu.MainMenu_C"));
    
    if(WidgetClass)
    {
        UE_LOG(LogTemp, Warning, TEXT("Widget class loaded!"));
        MainMenuWidget = CreateWidget<UUserWidget>(this, WidgetClass);
        
        if(MainMenuWidget)
        {
            UE_LOG(LogTemp, Warning, TEXT("Widget created! Adding to viewport..."));
            MainMenuWidget->AddToViewport(0);
            
            bShowMouseCursor = true;
            SetInputMode(FInputModeUIOnly());
            UE_LOG(LogTemp, Warning, TEXT("Done!"));
        }
        else
        {
            UE_LOG(LogTemp, Warning, TEXT("Failed to create widget instance!"));
        }
    }
    else
    {
        UE_LOG(LogTemp, Warning, TEXT("Failed to load widget class!"));
    }
}