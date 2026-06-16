import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OpenEventComponent } from './open-event.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing'; // לפי הצורך


describe('OpenEventComponent', () => {
  let component: OpenEventComponent;
  let fixture: ComponentFixture<OpenEventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OpenEventComponent, 
        HttpClientTestingModule, 
        RouterTestingModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OpenEventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
