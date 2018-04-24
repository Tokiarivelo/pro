import { Component } from '@angular/core';

@Component({ selector: "my-app", template: "<h1>{{welcomeMessage}}</h1>" })

export class AppComponent {
    welcomeMessage: string = "Welcome To Angular 2 mvc project";
}