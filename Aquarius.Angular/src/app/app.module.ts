import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { HttpClientModule } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { FarmComponent } from './Components/farm/farm.component';
import { PondComponent } from './Components/pond/pond.component';
import { SensorComponent } from './Components/sensor/sensor.component';
import { ReadingComponent } from './Components/reading/reading.component';

@NgModule({
  declarations: [
    AppComponent  // <-- Solo declara AppComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    HttpClientModule,
    MatButtonModule,
    MatListModule,
    FarmComponent,  // <-- Importa los componentes standalone aquí
    PondComponent,
    SensorComponent,
    ReadingComponent
  ],
  providers: [],
  bootstrap: [AppComponent]  // <-- Define el componente raíz
})
export class AppModule { }