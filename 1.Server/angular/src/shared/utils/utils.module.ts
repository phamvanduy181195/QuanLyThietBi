import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FileDownloadService } from './file-download.service';
import { ButtonBusyDirective } from './button-busy.directive';

@NgModule({
    imports: [
        CommonModule
    ],
    providers: [
        FileDownloadService
    ],
    declarations: [
        ButtonBusyDirective
    ],
    exports: [
        ButtonBusyDirective
    ]
})
export class UtilsModule { }