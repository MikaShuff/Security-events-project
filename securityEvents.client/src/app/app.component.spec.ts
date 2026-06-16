import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('AppComponent (standalone)', () => {
  it('should create the app', async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent, 
        HttpClientTestingModule, 
        RouterTestingModule],
    }).compileComponents();

    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;

    expect(component).toBeTruthy();
  });
});