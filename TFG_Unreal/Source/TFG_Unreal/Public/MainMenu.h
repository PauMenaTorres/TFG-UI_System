// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "MainMenu.generated.h"

/**
 * 
 */
UCLASS()
class TFG_UNREAL_API UMainMenu : public UUserWidget
{
	GENERATED_BODY()

	private: 

		virtual void NativeConstruct() override;

		UFUNCTION() 
		void OnStartClicked();

		UFUNCTION() 
		void OnOptionsClicked();

		UFUNCTION() 
		void OnExitClicked();

		UPROPERTY(meta = (BindWidget))
		class UButton* startButton;

		UPROPERTY(meta = (BindWidget))
		class UButton* optionsButton;

		UPROPERTY(meta = (BindWidget))
		class UButton* exitButton;
	
};
